package com.studhub.dataserver.grpc.service;

import com.google.protobuf.Struct;
import com.google.protobuf.Value;
import com.studhub.dataserver.inventory.InventoryServiceGrpc;
import com.studhub.dataserver.inventory.SetBrickLinkInventoryRequest;
import com.studhub.dataserver.inventory.SetBrickLinkInventoryResponse;
import com.studhub.dataserver.model.dto.InventoryDTO;
import com.studhub.dataserver.model.entity.Stud;
import com.studhub.dataserver.model.mapper.InventoryMapper;
import com.studhub.dataserver.repository.BricklinkInventoryRepository;
import com.studhub.dataserver.repository.StudRepository;
import io.grpc.stub.StreamObserver;
import net.devh.boot.grpc.server.service.GrpcService;

@GrpcService
public class BrinklinkInventoryService extends InventoryServiceGrpc.InventoryServiceImplBase {

    private final BricklinkInventoryRepository blInventoryRepo;
    private final StudRepository studRepository;
    private final InventoryMapper inventoryMapper;

    public BrinklinkInventoryService(BricklinkInventoryRepository blInventoryRepo, StudRepository studRepository, InventoryMapper inventoryMapper) {
        this.blInventoryRepo = blInventoryRepo;
        this.studRepository = studRepository;
        this.inventoryMapper = inventoryMapper;
    }

    @Override
    public void setBrickLinkInventories(SetBrickLinkInventoryRequest request,
                                        StreamObserver<SetBrickLinkInventoryResponse> responseObserver) {

        Integer userId = request.getUserId();
        Stud stud = studRepository.findById(userId)
                .orElseThrow(() -> new RuntimeException("User not found"));

        try {
            for (Struct s : request.getInventoriesList()) {
                InventoryDTO dto = inventoryMapper.fromStruct(s, userId);
                blInventoryRepo.upsertInventory(dto.getInventoryId(), dto.getJson(), dto.getUserId());
                // Convert struct → JSON string
//                String json = JsonFormat.printer().print(s);

                // Extract "inventory_id"
//                long inventoryId = getInventoryId(s);

                // Upsert to database
//                blInventoryRepo.upsertInventory(inventoryId, json, stud.getId());
            }

            SetBrickLinkInventoryResponse response = SetBrickLinkInventoryResponse.newBuilder()
                    .setIsSuccess(true)
                    .setMessage("Inventories saved successfully for user " + stud.getUsername())
                    .build();

            responseObserver.onNext(response);
            responseObserver.onCompleted();

        } catch (Exception e) {
            SetBrickLinkInventoryResponse error = SetBrickLinkInventoryResponse.newBuilder()
                    .setIsSuccess(false)
                    .setErrorMessage(e.getMessage())
                    .build();

            responseObserver.onNext(error);
            responseObserver.onCompleted();
        }
    }

    private long getInventoryId(Struct s) {
        Value val = s.getFieldsMap().get("inventory_id");
        if (val == null) throw new IllegalArgumentException("inventory_id missing");
        return (long) val.getNumberValue();
    }
}
