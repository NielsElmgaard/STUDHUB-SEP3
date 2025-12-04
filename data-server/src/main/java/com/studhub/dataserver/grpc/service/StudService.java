package com.studhub.dataserver.grpc.service;

import com.studhub.dataserver.common.UserId;
import com.studhub.dataserver.model.entity.BrickLinkConnection;
import com.studhub.dataserver.model.entity.BrickOwlConnection;
import com.studhub.dataserver.model.entity.Stud;
import com.studhub.dataserver.repository.StudRepository;
import com.studhub.dataserver.stud.*;
import io.grpc.stub.StreamObserver;
import net.devh.boot.grpc.server.service.GrpcService;
import org.hibernate.exception.ConstraintViolationException;
import org.springframework.dao.DataIntegrityViolationException;
import org.springframework.security.crypto.password.PasswordEncoder;

@GrpcService
public class StudService
        extends StudServiceGrpc.StudServiceImplBase {

    private final StudRepository studRepository;
    private final PasswordEncoder passwordEncoder;

    public StudService(StudRepository studRepository,
                       PasswordEncoder passwordEncoder) {
        this.studRepository = studRepository;
        this.passwordEncoder = passwordEncoder;
    }

    @Override
    public void createStud(CreateStudRequest request,
                           StreamObserver<CreateStudResponse> responseObserver) {

        // Hash password before saving
        Stud stud = new Stud();
        stud.setEmail(request.getEmail());
        stud.setUsername(request.getUsername());
        stud.setPasswordHash(passwordEncoder.encode(request.getPassword()));

        // Empty connection objects for new users
        BrickLinkConnection blConnection = new BrickLinkConnection();
        BrickOwlConnection boConnection = new BrickOwlConnection();

        stud.setBrickLinkConnection(blConnection);
        blConnection.setStud(stud);

        stud.setBrickOwlConnection(boConnection);
        boConnection.setStud(stud);

        try {
            Stud savedStud = studRepository.save(stud);

            CreateStudResponse response = CreateStudResponse.newBuilder()
                    .setIsSuccess(true).setId(savedStud.getId()).build();
            responseObserver.onNext(response);
            responseObserver.onCompleted();

        }
        // TODO: handle exception when create stud fail
        catch (DataIntegrityViolationException e) {
            if (e.getCause() instanceof ConstraintViolationException) {
                String detail = ((ConstraintViolationException) e.getCause()).getSQLException()
                        .getMessage();
                throw new io.grpc.StatusRuntimeException(
                        io.grpc.Status.ALREADY_EXISTS.withDescription(
                                "A stud user with this email already exists."));
            }
            throw new io.grpc.StatusRuntimeException(
                    io.grpc.Status.UNKNOWN.withCause(e));
        } catch (Exception e) {
            throw new io.grpc.StatusRuntimeException(
                    io.grpc.Status.UNKNOWN.withCause(e));
        }
    }

    @Override
    public void getStudByEmail(GetStudByEmailRequest request,
                               StreamObserver<GetStudByEmailResponse> responseObserver) {
        String email = request.getEmail();
        String password = request.getPassword();

        GetStudByEmailResponse.Builder responseBuilder = GetStudByEmailResponse.newBuilder();

        // Lookup user by email
        studRepository.findByEmail(email).ifPresentOrElse(stud -> {
            // Compare password
            if (passwordEncoder.matches(password, stud.getPasswordHash())) {
                responseBuilder.setId(stud.getId()).setEmail(stud.getEmail())
                        .setUsername(stud.getUsername()).setErrorMessage("");
            } else {
                responseBuilder.setId(-1).setEmail("").setUsername("")
                        .setErrorMessage("Invalid password");
            }
        }, () -> {
            responseBuilder.setId(-1).setEmail("").setUsername("")
                    .setErrorMessage("User not found");
        });

        responseObserver.onNext(responseBuilder.build());
        responseObserver.onCompleted();
    }

    @Override
    public void getStudById(GetStudByIdRequest request,
                            StreamObserver<GetStudByIdResponse> responseObserver) {
        Integer id = request.getId();
        String password = request.getPassword();

        GetStudByIdResponse.Builder responseBuilder = GetStudByIdResponse.newBuilder();

        // Lookup user by id
        studRepository.findById(id).ifPresentOrElse(stud -> {
            // Compare password
            if (passwordEncoder.matches(password, stud.getPasswordHash())) {
                responseBuilder.setUsername(stud.getUsername()).setErrorMessage("");
            } else {
                responseBuilder.setUsername("").setErrorMessage("Invalid password");
            }
        }, () -> {
            responseBuilder.setUsername("").setErrorMessage("User not found");
        });

        responseObserver.onNext(responseBuilder.build());
        responseObserver.onCompleted();
    }

    @Override
    public void getBrickLinkAuthById(
            UserId request,
            StreamObserver<GetBrickLinkAuthByIdResponse> responseObserver) {
        Integer id = request.getId();

        GetBrickLinkAuthByIdResponse.Builder b = GetBrickLinkAuthByIdResponse.newBuilder();

        studRepository.findById(id).ifPresentOrElse(stud -> {
            BrickLinkConnection connection = stud.getBrickLinkConnection();

            if (connection == null) {
                b.setConsumerKey("").setConsumerSecret("").setTokenValue("")
                        .setTokenSecret("");
                return;
            }

            b.setConsumerKey(connection.getConsumerKey() == null ?
                            "" :
                            connection.getConsumerKey()).setConsumerSecret(
                            connection.getConsumerSecret() == null ?
                                    "" :
                                    connection.getConsumerSecret()).setTokenValue(
                            connection.getTokenValue() == null ? "" : connection.getTokenValue())
                    .setTokenSecret(connection.getTokenSecret() == null ?
                            "" :
                            connection.getTokenSecret());
        }, () -> {
            b.setConsumerKey("").setConsumerSecret("").setTokenValue("")
                    .setTokenSecret("");
        });

        responseObserver.onNext(b.build());
        responseObserver.onCompleted();
    }

    @Override
    public void getBrickOwlAuthById(UserId request,
                                    StreamObserver<GetBrickOwlAuthByIdResponse> responseObserver) {
        Integer id = request.getId();

        GetBrickOwlAuthByIdResponse.Builder b = GetBrickOwlAuthByIdResponse.newBuilder();

        studRepository.findById(id).ifPresentOrElse(stud -> {
            BrickOwlConnection connection = stud.getBrickOwlConnection();

            if (connection == null) {
                b.setApiKey("");
                return;
            }

            b.setApiKey(connection.getApiKey() == null ? "" : connection.getApiKey());
        }, () -> {
            b.setApiKey("");
        });

        responseObserver.onNext(b.build());
        responseObserver.onCompleted();
    }

    @Override
    public void setBrickLinkAuthById(
            SetBrickLinkAuthByIdRequest request,
            StreamObserver<SetBrickLinkAuthByIdResponse> responseObserver) {
        Integer id = request.getId();

        SetBrickLinkAuthByIdResponse.Builder res = SetBrickLinkAuthByIdResponse.newBuilder();

        try {
            var optionalStud = studRepository.findById(id);
            if (optionalStud.isEmpty()) {
                res.setIsSuccess(false).setErrorMessage("User not found");
            } else {
                var stud = optionalStud.get();
                BrickLinkConnection connection = stud.getBrickLinkConnection();

                if (connection == null) {
                    connection = new BrickLinkConnection();
                    stud.setBrickLinkConnection(connection);
                    connection.setStud(stud);
                }

                connection.setConsumerKey(request.getConsumerKey());
                connection.setConsumerSecret(request.getConsumerSecret());
                connection.setTokenValue(request.getTokenValue());
                connection.setTokenSecret(request.getTokenSecret());
                studRepository.save(stud);

                res.setIsSuccess(true).setErrorMessage("");
            }
        } catch (Exception e) {
            res.setIsSuccess(false).setErrorMessage(e.getMessage());
        }

        responseObserver.onNext(res.build());
        responseObserver.onCompleted();
    }

    @Override
    public void setBrickOwlAuthById(SetBrickOwlAuthByIdRequest request,
                                    StreamObserver<SetBrickOwlAuthByIdResponse> responseObserver) {
        Integer id = request.getId();

        SetBrickOwlAuthByIdResponse.Builder res = SetBrickOwlAuthByIdResponse.newBuilder();

        try {
            var optionalStud = studRepository.findById(id);
            if (optionalStud.isEmpty()) {
                res.setIsSuccess(false).setErrorMessage("User not found");
            } else {
                var stud = optionalStud.get();
                BrickOwlConnection connection = stud.getBrickOwlConnection();

                if (connection == null) {
                    connection = new BrickOwlConnection();
                    stud.setBrickOwlConnection(connection);
                    connection.setStud(stud);
                }

                connection.setApiKey(request.getApiKey());
                studRepository.save(stud);

                res.setIsSuccess(true).setErrorMessage("");
            }
        } catch (Exception e) {
            res.setIsSuccess(false).setErrorMessage(e.getMessage());
        }

        responseObserver.onNext(res.build());
        responseObserver.onCompleted();
    }
}
