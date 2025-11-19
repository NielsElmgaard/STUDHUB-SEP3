package com.studhub.dataserver;

import com.studhub.dataserver.storeconnection.BrickLinkConnection;
import com.studhub.dataserver.storeconnection.BrickOwlConnection;
import jakarta.persistence.*;

@Entity public class Stud
{
  @Id @GeneratedValue(strategy = GenerationType.IDENTITY) private Long id;

  @Column(nullable = false, unique = true) private String email;

  @Column(nullable = false) private String username;
  @Column(nullable = false) private String passwordHash;

  @OneToOne(mappedBy = "stud", cascade = CascadeType.ALL, orphanRemoval = true)
  private BrickLinkConnection brickLinkConnection;

  @OneToOne(mappedBy = "stud", cascade = CascadeType.ALL, orphanRemoval = true)
  private BrickOwlConnection brickOwlConnection;

  public BrickLinkConnection getBrickLinkConnection()
  {
    return brickLinkConnection;
  }

  public void setBrickLinkConnection(BrickLinkConnection brickLinkConnection)
  {
    this.brickLinkConnection = brickLinkConnection;
  }

  public BrickOwlConnection getBrickOwlConnection()
  {
    return brickOwlConnection;
  }

  public void setBrickOwlConnection(BrickOwlConnection brickOwlConnection)
  {
    this.brickOwlConnection = brickOwlConnection;
  }

  public Long getId()
  {
    return id;
  }

  // Should only be used for testing
  public void setId(Long id)
  {
    this.id = id;
  }

  public String getEmail()
  {
    return email;
  }

  public void setEmail(String email)
  {
    this.email = email;
  }

  public String getUsername()
  {
    return username;
  }

  public void setUsername(String username)
  {
    this.username = username;
  }

  public String getPasswordHash()
  {
    return passwordHash;
  }

  public void setPasswordHash(String passwordHash)
  {
    this.passwordHash = passwordHash;
  }


}

