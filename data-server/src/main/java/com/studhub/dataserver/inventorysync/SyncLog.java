package com.studhub.dataserver.inventorysync;

import com.studhub.dataserver.Stud;
import jakarta.persistence.*;

import java.time.Instant;

@Entity public class SyncLog
{
  @Id @GeneratedValue(strategy = GenerationType.IDENTITY) private Long logId;

  @ManyToOne(fetch = FetchType.LAZY) @JoinColumn(name = "session_id", nullable = false) private InventorySyncSession session; // FK

  @Column(nullable = false) private Instant timestamp;

  @Enumerated(EnumType.STRING) @Column(nullable = false) private SyncLogType type;

  /*
  @ManyToOne(fetch = FetchType.LAZY) @JoinColumn(name = "inventory_lot_id") private InventoryLot inventoryLot; // FK
   */

  @Column private String errorMessage;

  public void setLogId(Long logId)
  {
    this.logId = logId;
  }

  public SyncLog()
  {
  }

  public void setSession(InventorySyncSession session)
  {
    this.session = session;
  }

  public void setTimestamp(Instant timestamp)
  {
    this.timestamp = timestamp;
  }

  public void setType(SyncLogType type)
  {
    this.type = type;
  }

  /*
  public void setInventoryLot(InventoryLot inventoryLot)
  {
    this.inventoryLot = inventoryLot;
  }
  */

  public void setErrorMessage(String errorMessage)
  {
    this.errorMessage = errorMessage;
  }

  public Long getLogId()
  {
    return logId;
  }

  public InventorySyncSession getSession()
  {
    return session;
  }

  public Instant getTimestamp()
  {
    return timestamp;
  }

  public SyncLogType getType()
  {
    return type;
  }

  /*
  public InventoryLot getInventoryLot()
  {
    return inventoryLot;
  }
  */

  public String getErrorMessage()
  {
    return errorMessage;
  }
}
