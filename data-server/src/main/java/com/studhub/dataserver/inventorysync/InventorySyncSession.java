package com.studhub.dataserver.inventorysync;

import com.studhub.dataserver.Stud;
import jakarta.persistence.*;

import java.time.Instant;

@Entity public class InventorySyncSession
{
  @Id @GeneratedValue(strategy = GenerationType.IDENTITY) private Long sessionId;

  @ManyToOne(fetch = FetchType.LAZY) @JoinColumn(name = "stud_id", nullable = false) private Stud stud; // FK

  @Column(nullable = false) private Instant startTimestamp;

  @Column private Instant endTimestamp;

  @Enumerated(EnumType.STRING) @Column(nullable = false) private SyncStatus status;

  @Column(name = "items_updated_to_brickowl_count") private int itemsUpdatedToBrickOwlCount=0;

  @Column(name = "items_fetched_from_bricklink_count") private int itemsFetchedFromBrickLinkCount=0;

  @Column private Instant lastSuccessfulSyncTime;

  @Column(nullable = false) private int errorCount;

  @Column(nullable = false) private boolean isManual = false;

  public InventorySyncSession(){}

  public void setSessionId(Long sessionId)
  {
    this.sessionId = sessionId;
  }

  public void setStud(Stud stud)
  {
    this.stud = stud;
  }

  public void setStartTimestamp(Instant startTimestamp)
  {
    this.startTimestamp = startTimestamp;
  }

  public void setEndTimestamp(Instant endTimestamp)
  {
    this.endTimestamp = endTimestamp;
  }

  public void setStatus(SyncStatus status)
  {
    this.status = status;
  }

  public void setItemsUpdatedToBrickOwlCount(int itemsUpdatedToBrickOwlCount)
  {
    this.itemsUpdatedToBrickOwlCount = itemsUpdatedToBrickOwlCount;
  }

  public void setItemsFetchedFromBrickLinkCount(
      int itemsFetchedFromBrickLinkCount)
  {
    this.itemsFetchedFromBrickLinkCount = itemsFetchedFromBrickLinkCount;
  }

  public void setLastSuccessfulSyncTime(Instant lastSuccessfulSyncTime)
  {
    this.lastSuccessfulSyncTime = lastSuccessfulSyncTime;
  }

  public void setErrorCount(int errorCount)
  {
    this.errorCount = errorCount;
  }

  public void setManual(boolean manual)
  {
    isManual = manual;
  }

  public Long getSessionId()
  {
    return sessionId;
  }

  public Stud getStud()
  {
    return stud;
  }

  public Instant getStartTimestamp()
  {
    return startTimestamp;
  }

  public Instant getEndTimestamp()
  {
    return endTimestamp;
  }

  public SyncStatus getStatus()
  {
    return status;
  }

  public int getItemsUpdatedToBrickOwlCount()
  {
    return itemsUpdatedToBrickOwlCount;
  }

  public int getItemsFetchedFromBrickLinkCount()
  {
    return itemsFetchedFromBrickLinkCount;
  }

  public Instant getLastSuccessfulSyncTime()
  {
    return lastSuccessfulSyncTime;
  }

  public int getErrorCount()
  {
    return errorCount;
  }

  public boolean isManual()
  {
    return isManual;
  }
}
