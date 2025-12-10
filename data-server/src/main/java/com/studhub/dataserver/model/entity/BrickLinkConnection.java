package com.studhub.dataserver.model.entity;

import com.studhub.dataserver.config.EncryptionConverter;
import jakarta.persistence.*;

@Entity public class BrickLinkConnection
{
  @Id private int id; // PK
  @Convert(converter = EncryptionConverter.class) private String consumerKey;
  @Convert(converter = EncryptionConverter.class) private String consumerSecret;
  @Convert(converter = EncryptionConverter.class) private String tokenValue;
  @Convert(converter = EncryptionConverter.class) private String tokenSecret;
  @OneToOne @MapsId @JoinColumn(name = "stud_id") private Stud stud; // FK

  public Stud getStud()
  {
    return stud;
  }

  public void setStud(Stud stud)
  {
    this.stud = stud;
  }

  public int getId()
  {
    return id;
  }

  // Should only be used for testing
  public void setId(int id)
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
