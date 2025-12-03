package com.studhub.dataserver.storeconnection;

import com.studhub.dataserver.model.entity.Stud;
import jakarta.persistence.*;

@Entity public class BrickLinkConnection
{
  @Id private Long id; // PK
  private String consumerKey;
  private String consumerSecret;
  private String tokenValue;
  private String tokenSecret;
  @OneToOne @MapsId @JoinColumn(name = "stud_id") private Stud stud; // FK

  public Stud getStud()
  {
    return stud;
  }

  public void setStud(Stud stud)
  {
    this.stud = stud;
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

  public String getConsumerKey()
  {
    return consumerKey;
  }

  public void setConsumerKey(String consumerKey)
  {
    this.consumerKey = consumerKey;
  }

  public String getConsumerSecret()
  {
    return consumerSecret;
  }

  public void setConsumerSecret(String consumerSecret)
  {
    this.consumerSecret = consumerSecret;
  }

  public String getTokenSecret()
  {
    return tokenSecret;
  }

  public void setTokenSecret(String tokenSecret)
  {
    this.tokenSecret = tokenSecret;
  }

  public String getTokenValue()
  {
    return tokenValue;
  }

  public void setTokenValue(String tokenValue)
  {
    this.tokenValue = tokenValue;
  }
}
