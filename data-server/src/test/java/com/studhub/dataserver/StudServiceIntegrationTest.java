// https://gemini.google.com/ 18-12-2025

package com.studhub.dataserver;

import com.studhub.dataserver.grpc.service.StudService;
import com.studhub.dataserver.model.entity.Stud;
import com.studhub.dataserver.repository.StudRepository;
import com.studhub.dataserver.stud.SetBrickLinkAuthByIdRequest;
import com.studhub.dataserver.stud.SetBrickLinkAuthByIdResponse;
import org.junit.jupiter.api.Test;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.test.context.ActiveProfiles;

import static org.junit.jupiter.api.Assertions.*;

@SpringBootTest
@ActiveProfiles("test") // Loads application-test.properties
class StudServiceIntegrationTest {

  @Autowired
  private StudRepository studRepository;

  @Autowired
  private StudService studService; // gRPC service implementation

  @Test
  void testSetBrickLinkAuth_Success() {
    // --- Arrange ---
    // Prepare the state: An existing user must be in the database
    Stud stud = new Stud();
    stud.setUsername("testuser");
    stud.setEmail("test@studhub.dk");
    stud.setPasswordHash("dummy_hash");
    stud = studRepository.saveAndFlush(stud);

    // POINT 4: Prepare the gRPC request arriving from the App Server
    SetBrickLinkAuthByIdRequest request = SetBrickLinkAuthByIdRequest.newBuilder()
        .setId(stud.getId())
        .setConsumerKey("test-consumer-key")
        .setConsumerSecret("test-secret")
        .setTokenValue("test-token")
        .setTokenSecret("test-token-secret")
        .build();

    TestResponseObserver<SetBrickLinkAuthByIdResponse> responseObserver = new TestResponseObserver<>();

    // --- Act ---
    // POINT 4: Execute the service call
    studService.setBrickLinkAuthById(request, responseObserver);

    // --- Assert ---
    // POINT 4.3: Verify response.setSuccess(true)
    assertTrue(responseObserver.getSingleResponse().getIsSuccess());

    // POINT 4.1 & 4.2: Verify that POINT 4.2 (studRepository.save) actually persisted the data
    Stud updatedStud = studRepository.findById(stud.getId()).orElseThrow();
    assertNotNull(updatedStud.getBrickLinkConnection());
    assertEquals("test-consumer-key", updatedStud.getBrickLinkConnection().getConsumerKey());
  }

  @Test
  void testSetBrickLinkAuth_UserNotFound() {
    // --- Arrange ---
    // RAINY DAY: Point 4 is called with an ID that does not exist in the database
    SetBrickLinkAuthByIdRequest request = SetBrickLinkAuthByIdRequest.newBuilder()
        .setId(9999)
        .setConsumerKey("key")
        .build();
    TestResponseObserver<SetBrickLinkAuthByIdResponse> responseObserver = new TestResponseObserver<>();

    // --- Act ---
    studService.setBrickLinkAuthById(request, responseObserver);

    // --- Assert ---
    // Verify that the Data Server returns failure instead of crashing when user is missing
    assertFalse(responseObserver.getSingleResponse().getIsSuccess());
  }

  private static class TestResponseObserver<T> implements io.grpc.stub.StreamObserver<T> {
    private T response;
    private Throwable error;
    private boolean completed = false;

    @Override
    public void onNext(T value) {
      this.response = value;
    }

    @Override
    public void onError(Throwable t) {
      this.error = t;
    }

    @Override
    public void onCompleted() {
      this.completed = true;
    }

    public T getSingleResponse() {
      if (error != null) throw new RuntimeException(error);
      return response;
    }
  }
}


