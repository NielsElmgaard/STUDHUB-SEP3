package com.studhub.dataserver;

import io.grpc.stub.StreamObserver;
import net.devh.boot.grpc.server.service.GrpcService;
import org.springframework.security.crypto.password.PasswordEncoder;

@GrpcService
public class StudServiceImpl extends StudServiceGrpc.StudServiceImplBase {

    private final StudRepository studRepository;
    private final PasswordEncoder passwordEncoder;

    public StudServiceImpl(StudRepository studRepository, PasswordEncoder passwordEncoder) {
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

        studRepository.save(stud);

        // TODO: handle exception when create stud fail
        CreateStudResponse response = CreateStudResponse.newBuilder()
                .setIsSuccess(true)
                .build();
        responseObserver.onNext(response);
        responseObserver.onCompleted();
    }

    @Override
    public void getStudByEmail(GetStudByEmailRequest request, StreamObserver<GetStudByEmailResponse> responseObserver) {
        String email = request.getEmail();
        String password = request.getPassword();

        GetStudByEmailResponse.Builder responseBuilder = GetStudByEmailResponse.newBuilder();

        // Lookup user by email
        studRepository.findByEmail(email).ifPresentOrElse(stud -> {
            // Compare password
            if (passwordEncoder.matches(password, stud.getPasswordHash())) {
                responseBuilder.setUsername(stud.getUsername())
                        .setErrorMessage("");
            } else {
                responseBuilder.setUsername("")
                        .setErrorMessage("Invalid password");
            }
        }, () -> {
            responseBuilder.setUsername("")
                    .setErrorMessage("User not found");
        });

        responseObserver.onNext(responseBuilder.build());
        responseObserver.onCompleted();
    }

    @Override
    public void getAuthByEmail(GetAuthByEmailRequest request,
                               StreamObserver<GetAuthByEmailResponse> responseObserver) {
        String email = request.getEmail();

        GetAuthByEmailResponse.Builder b = GetAuthByEmailResponse.newBuilder();

        studRepository.findByEmail(email).ifPresentOrElse(stud -> {
            b.setConsumerKey(stud.getConsumerKey() == null ? "" : stud.getConsumerKey())
                    .setConsumerSecret(stud.getConsumerSecret() == null ? "" : stud.getConsumerSecret())
                    .setTokenValue(stud.getTokenValue() == null ? "" : stud.getTokenValue())
                    .setTokenSecret(stud.getTokenSecret() == null ? "" : stud.getTokenSecret());
        }, () -> {
            b.setConsumerKey("").setConsumerSecret("").setTokenValue("").setTokenSecret("");
        });

        responseObserver.onNext(b.build());
        responseObserver.onCompleted();
    }

    @Override
    public void setAuthByEmail(SetAuthByEmailRequest request,
                               StreamObserver<SetAuthByEmailResponse> responseObserver) {
        String email = request.getEmail();

        SetAuthByEmailResponse.Builder res = SetAuthByEmailResponse.newBuilder();

        try {
            var optionalStud = studRepository.findByEmail(email);
            if (optionalStud.isEmpty()) {
                res.setIsSuccess(false)
                        .setErrorMessage("User not found");
            } else {
                var stud = optionalStud.get();
                stud.setConsumerKey(request.getConsumerKey());
                stud.setConsumerSecret(request.getConsumerSecret());
                stud.setTokenValue(request.getTokenValue());
                stud.setTokenSecret(request.getTokenSecret());
                studRepository.save(stud);

                res.setIsSuccess(true)
                        .setErrorMessage("");
            }
        } catch (Exception e) {
            res.setIsSuccess(false)
                    .setErrorMessage(e.getMessage());
        }

        responseObserver.onNext(res.build());
        responseObserver.onCompleted();
    }
}
