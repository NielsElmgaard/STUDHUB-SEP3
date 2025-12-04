package com.studhub.dataserver.inventorysync;

import jakarta.persistence.*;

import java.time.Instant;

@Entity
public class SyncLog {
    @Id
    @GeneratedValue(strategy = GenerationType.IDENTITY)
    private Long logId;

    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "session_id", nullable = false)
    private InventorySyncSession session; // FK

    @Column(nullable = false)
    private Instant timestamp;

    @Enumerated(EnumType.STRING)
    @Column(nullable = false)
    private SyncLogType type;

  /*
  @ManyToOne(fetch = FetchType.LAZY) @JoinColumn(name = "inventory_lot_id") private InventoryLot inventoryLot; // FK
   */

    @Column
    private String errorMessage;

    public SyncLog() {
    }

    public Long getLogId() {
        return logId;
    }

    public void setLogId(Long logId) {
        this.logId = logId;
    }

    public InventorySyncSession getSession() {
        return session;
    }

    public void setSession(InventorySyncSession session) {
        this.session = session;
    }

  /*
  public void setInventoryLot(InventoryLot inventoryLot)
  {
    this.inventoryLot = inventoryLot;
  }
  */

    public Instant getTimestamp() {
        return timestamp;
    }

    public void setTimestamp(Instant timestamp) {
        this.timestamp = timestamp;
    }

    public SyncLogType getType() {
        return type;
    }

    public void setType(SyncLogType type) {
        this.type = type;
    }

    public String getErrorMessage() {
        return errorMessage;
    }

  /*
  public InventoryLot getInventoryLot()
  {
    return inventoryLot;
  }
  */

    public void setErrorMessage(String errorMessage) {
        this.errorMessage = errorMessage;
    }
}
