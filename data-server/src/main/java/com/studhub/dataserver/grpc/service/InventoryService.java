package com.studhub.dataserver.grpc.service;

import com.google.protobuf.Struct;
import com.google.protobuf.Value;
import com.studhub.dataserver.common.DataSource;
import com.studhub.dataserver.common.UpdateRequest;
import com.studhub.dataserver.common.UpdateResponse;
import com.studhub.dataserver.common.UserId;
import com.studhub.dataserver.inventory.BrickOwlDiffResponse;
import com.studhub.dataserver.inventory.BrickowlInventory;
import com.studhub.dataserver.inventory.InventoryServiceGrpc;
import com.studhub.dataserver.model.dto.InventoryDTO;
import com.studhub.dataserver.model.entity.Stud;
import com.studhub.dataserver.model.mapper.InventoryMapper;
import com.studhub.dataserver.repository.*;
import com.studhub.dataserver.repository.projection.BrickOwlDiffProjection;
import io.grpc.stub.StreamObserver;
import net.devh.boot.grpc.server.service.GrpcService;

import java.util.List;

@GrpcService
public class InventoryService extends InventoryServiceGrpc.InventoryServiceImplBase {

    private final BricklinkInventoryRepository blInventoryRepo;
    private final BrickowlInventoryRepository boInventoryRepo;
    private final BlBoInventoryMapRepository blBoInventoryMapRepo;
    private final StudRepository studRepository;
    private final InventoryMapper inventoryMapper;

    public InventoryService(BricklinkInventoryRepository blInventoryRepo, BrickowlInventoryRepository boInventoryRepo,
                            BlBoInventoryMapRepository blBoInventoryMapRepo, StudRepository studRepository,
                            InventoryMapper inventoryMapper) {
        this.blInventoryRepo = blInventoryRepo;
        this.boInventoryRepo = boInventoryRepo;
        this.blBoInventoryMapRepo = blBoInventoryMapRepo;
        this.studRepository = studRepository;
        this.inventoryMapper = inventoryMapper;
    }


    @Override
    public void setInventories(UpdateRequest request,
                               StreamObserver<UpdateResponse> responseObserver) {

        Integer userId = request.getUserId();
        DataSource source = request.getSource();
        IInventoryRepository repository;

        if (source == DataSource.BRICKLINK) {
            repository = blInventoryRepo;
        } else if (source == DataSource.BRICKOWL) {
            repository = boInventoryRepo;
        } else {
            throw new IllegalArgumentException("Unknown inventory source: " + source);
        }

        Stud stud = studRepository.findById(userId)
                .orElseThrow(() -> new RuntimeException("User not found"));

        try {
            String idKey = (source == DataSource.BRICKLINK) ? "inventory_id" : "lot_id";

            for (Struct s : request.getDataList()) {
                Integer id = getInventoryId(s, idKey);
                InventoryDTO dto = inventoryMapper.fromStruct(s, userId, id);
                repository.upsertInventory(dto.getInventoryId(), dto.getJson(), dto.getUserId());
            }

            UpdateResponse response = UpdateResponse.newBuilder()
                    .setIsSuccess(true)
                    .setMessage("Inventories saved successfully for user " + stud.getUsername())
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();

        } catch (Exception e) {
            UpdateResponse error = UpdateResponse.newBuilder()
                    .setIsSuccess(false)
                    .setErrorMessage(e.getMessage())
                    .build();

            responseObserver.onNext(error);
            responseObserver.onCompleted();
        }
    }

    private Integer getInventoryId(Struct s, String idKey) {
        Value val = s.getFieldsMap().get(idKey);
        if (val == null) throw new IllegalArgumentException(idKey + " missing in inventory data");

        Integer id;

        switch (val.getKindCase()) {
            case NUMBER_VALUE -> id = (int) val.getNumberValue();
            case STRING_VALUE -> {
                try {
                    id = Integer.parseInt(val.getStringValue());
                } catch (NumberFormatException e) {
                    throw new IllegalArgumentException(idKey + " is not a valid number: " + val.getStringValue());
                }
            }
            default -> throw new IllegalArgumentException(idKey + " must be a number or numeric string");
        }

        return id;
    }

    @Override
    public void getDiffInventoryForBrickOwl(UserId request,
                                            StreamObserver<BrickOwlDiffResponse> responseObserver) {

        try {
            Integer userId = request.getId();
            Stud stud = studRepository.findById(userId)
                    .orElseThrow(() -> new RuntimeException("User not found"));

            List<BrickOwlDiffProjection> boDiffs = blInventoryRepo.findAllForBrickOwl(stud.getId());

            BrickOwlDiffResponse.Builder responseBuilder = BrickOwlDiffResponse.newBuilder();
            for (var diff : boDiffs) {
                BrickowlInventory builder = BrickowlInventory.newBuilder()
                        .setType(diff.getType() != null ? diff.getType() : "")
                        .setQuantity(diff.getQuantity() != null ? diff.getQuantity() : "0.0")
                        .setPrice(diff.getPrice() != null ? diff.getPrice() : "0.00")
                        .setBoid(diff.getBoid() != null ? diff.getBoid() : "")
                        .setLotId(diff.getLotId() != null ? diff.getLotId() : "")
                        .setAction(diff.getAction() != null ? diff.getAction() : "")
                        .build();

                responseBuilder.addDiffs(builder);
            }
            responseObserver.onNext(responseBuilder.setIsSuccess(true).build());
            responseObserver.onCompleted();
        } catch (Exception e) {
            e.printStackTrace();
            BrickOwlDiffResponse error = BrickOwlDiffResponse.newBuilder()
                    .setIsSuccess(false)
                    .setErrorMessage(e.getMessage() != null ? e.getMessage() : "Unknown error").build();

            responseObserver.onNext(error);
            responseObserver.onCompleted();
        }
    }
}
