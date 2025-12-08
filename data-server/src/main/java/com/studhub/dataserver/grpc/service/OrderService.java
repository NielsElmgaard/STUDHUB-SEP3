package com.studhub.dataserver.grpc.service;

import com.google.protobuf.Struct;
import com.google.protobuf.Value;
import com.studhub.dataserver.common.DataSource;
import com.studhub.dataserver.common.UpdateRequest;
import com.studhub.dataserver.common.UpdateResponse;
import com.studhub.dataserver.model.dto.OrderDTO;
import com.studhub.dataserver.model.entity.Stud;
import com.studhub.dataserver.model.mapper.OrderMapper;
import com.studhub.dataserver.order.OrderServiceGrpc;
import com.studhub.dataserver.repository.BrickLinkOrderRepository;
import com.studhub.dataserver.repository.BrickOwlOrderRepository;
import com.studhub.dataserver.repository.IOrderRepository;
import com.studhub.dataserver.repository.StudRepository;
import io.grpc.stub.StreamObserver;
import net.devh.boot.grpc.server.service.GrpcService;

@GrpcService
public class OrderService extends OrderServiceGrpc.OrderServiceImplBase {

    private final BrickLinkOrderRepository blOrderRepo;
    private final BrickOwlOrderRepository boOrderRepo;
    private final StudRepository studRepository;
    private final OrderMapper orderMapper;

    public OrderService(BrickLinkOrderRepository blOrderRepo, BrickOwlOrderRepository boOrderRepo, BrickOwlOrderRepository boOrderRepo1,
                        StudRepository studRepository, OrderMapper orderMapper) {
        this.blOrderRepo = blOrderRepo;
        this.boOrderRepo = boOrderRepo;
        this.studRepository = studRepository;
        this.orderMapper = orderMapper;
    }

    @Override
    public void updateOrders(UpdateRequest request,
                             StreamObserver<UpdateResponse> responseObserver) {
        Integer userId = request.getUserId();
        DataSource source = request.getSource();
        IOrderRepository repository;

        if (source == DataSource.BRICKLINK) {
            repository = blOrderRepo;
        } else if (source == DataSource.BRICKOWL) {
            repository = boOrderRepo;
        } else {
            throw new IllegalArgumentException("Unknown inventory source: " + source);
        }

        Stud stud = studRepository.findById(userId)
                .orElseThrow(() -> new RuntimeException("User not found"));

        try {
            String idKey = (source == DataSource.BRICKLINK) ? "order_id" : "?"; // TODO: find out order id field for brick owl

            for (Struct s : request.getDataList()) {
                Integer id = getOrderId(s, idKey);
                OrderDTO dto = orderMapper.fromStruct(s, userId, id);
                repository.upsertOrder(dto.getOrderId(), dto.getJson(), dto.getUserId());
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

    private Integer getOrderId(Struct s, String idKey) {
        Value val = s.getFieldsMap().get(idKey);
        if (val == null) throw new IllegalArgumentException(idKey + " missing in inventory data");
        return (int) val.getNumberValue();
//        Integer id;

//        switch (val.getKindCase()) {
//            case NUMBER_VALUE -> id = (int) val.getNumberValue();
//            case STRING_VALUE -> {
//                try {
//                    id = Integer.parseInt(val.getStringValue());
//                } catch (NumberFormatException e) {
//                    throw new IllegalArgumentException(idKey + " is not a valid number: " + val.getStringValue());
//                }
//            }
//            default -> throw new IllegalArgumentException(idKey + " must be a number or numeric string");
//        }

//        return id;
    }
}
