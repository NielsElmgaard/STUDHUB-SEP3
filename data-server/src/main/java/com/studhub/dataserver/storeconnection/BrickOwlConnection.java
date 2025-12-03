package com.studhub.dataserver.storeconnection;

import com.studhub.dataserver.model.entity.Stud;
import jakarta.persistence.*;

@Entity public class BrickOwlConnection
{
  @Id private Long id; // PK
  private String apiKey;
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

  public String getApiKey()
  {
    return apiKey;
  }

  public void setApiKey(String apiKey)
  {
    this.apiKey = apiKey;
  }
}
