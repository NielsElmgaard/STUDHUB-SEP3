package com.studhub.dataserver;

import com.studhub.dataserver.storeconnection.BrickLinkConnection;
import com.studhub.dataserver.storeconnection.BrickOwlConnection;
import io.grpc.stub.StreamObserver;
import net.devh.boot.grpc.server.service.GrpcService;
import org.springframework.security.crypto.password.PasswordEncoder;

@GrpcService public class StudServiceImpl
    extends StudServiceGrpc.StudServiceImplBase
{

  private final StudRepository studRepository;
  private final PasswordEncoder passwordEncoder;

  public StudServiceImpl(StudRepository studRepository,
      PasswordEncoder passwordEncoder)
  {
    this.studRepository = studRepository;
    this.passwordEncoder = passwordEncoder;
  }

  @Override public void createStud(CreateStudRequest request,
      StreamObserver<CreateStudResponse> responseObserver)
  {

    // Hash password before saving
    Stud stud = new Stud();
    stud.setEmail(request.getEmail());
    stud.setUsername(request.getUsername());
    stud.setPasswordHash(passwordEncoder.encode(request.getPassword()));

    studRepository.save(stud);

    // TODO: handle exception when create stud fail
    CreateStudResponse response = CreateStudResponse.newBuilder()
        .setIsSuccess(true).build();
    responseObserver.onNext(response);
    responseObserver.onCompleted();
  }

  @Override public void getStudByEmail(GetStudByEmailRequest request,
      StreamObserver<GetStudByEmailResponse> responseObserver)
  {
    String email = request.getEmail();
    String password = request.getPassword();

    GetStudByEmailResponse.Builder responseBuilder = GetStudByEmailResponse.newBuilder();

    // Lookup user by email
    studRepository.findByEmail(email).ifPresentOrElse(stud -> {
      // Compare password
      if (passwordEncoder.matches(password, stud.getPasswordHash()))
      {
        responseBuilder.setUsername(stud.getUsername()).setErrorMessage("");
      }
      else
      {
        responseBuilder.setUsername("").setErrorMessage("Invalid password");
      }
    }, () -> {
      responseBuilder.setUsername("").setErrorMessage("User not found");
    });

    responseObserver.onNext(responseBuilder.build());
    responseObserver.onCompleted();
  }

  @Override public void getBrickLinkAuthById(
      GetBrickLinkAuthByIdRequest request,
      StreamObserver<GetBrickLinkAuthByIdResponse> responseObserver)
  {
    Long id = request.getId();

    GetBrickLinkAuthByIdResponse.Builder b = GetBrickLinkAuthByIdResponse.newBuilder();

    studRepository.findById(id).ifPresentOrElse(stud -> {
      BrickLinkConnection connection = stud.getBrickLinkConnection();
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

  @Override public void getBrickOwlAuthById(GetBrickOwlAuthByIdRequest request,
      StreamObserver<GetBrickOwlAuthByIdResponse> responseObserver)
  {
    Long id = request.getId();

    GetBrickOwlAuthByIdResponse.Builder b = GetBrickOwlAuthByIdResponse.newBuilder();

    studRepository.findById(id).ifPresentOrElse(stud -> {
      BrickOwlConnection connection = stud.getBrickOwlConnection();
      b.setApiKey(connection.getApiKey() == null ? "" : connection.getApiKey());
    }, () -> {
      b.setApiKey("");
    });

    responseObserver.onNext(b.build());
    responseObserver.onCompleted();
  }

  @Override public void setBrickLinkAuthById(
      SetBrickLinkAuthByIdRequest request,
      StreamObserver<SetBrickLinkAuthByIdResponse> responseObserver)
  {
    Long id = request.getId();

    SetBrickLinkAuthByIdResponse.Builder res = SetBrickLinkAuthByIdResponse.newBuilder();

    try
    {
      var optionalStud = studRepository.findById(id);
      if (optionalStud.isEmpty())
      {
        res.setIsSuccess(false).setErrorMessage("User not found");
      }
      else
      {
        var stud = optionalStud.get();
        BrickLinkConnection connection = stud.getBrickLinkConnection();

        connection.setConsumerKey(request.getConsumerKey());
        connection.setConsumerSecret(request.getConsumerSecret());
        connection.setTokenValue(request.getTokenValue());
        connection.setTokenSecret(request.getTokenSecret());
        studRepository.save(stud);

        res.setIsSuccess(true).setErrorMessage("");
      }
    }
    catch (Exception e)
    {
      res.setIsSuccess(false).setErrorMessage(e.getMessage());
    }

    responseObserver.onNext(res.build());
    responseObserver.onCompleted();
  }

  @Override public void setBrickOwlAuthById(
      SetBrickOwlAuthByIdRequest request,
      StreamObserver<SetBrickOwlAuthByIdResponse> responseObserver)
  {
    Long id = request.getId();

    SetBrickOwlAuthByIdResponse.Builder res = SetBrickOwlAuthByIdResponse.newBuilder();

    try
    {
      var optionalStud = studRepository.findById(id);
      if (optionalStud.isEmpty())
      {
        res.setIsSuccess(false).setErrorMessage("User not found");
      }
      else
      {
        var stud = optionalStud.get();
        BrickOwlConnection connection = stud.getBrickOwlConnection();

        connection.setApiKey(request.getApiKey());
        studRepository.save(stud);

        res.setIsSuccess(true).setErrorMessage("");
      }
    }
    catch (Exception e)
    {
      res.setIsSuccess(false).setErrorMessage(e.getMessage());
    }

    responseObserver.onNext(res.build());
    responseObserver.onCompleted();
  }
}
