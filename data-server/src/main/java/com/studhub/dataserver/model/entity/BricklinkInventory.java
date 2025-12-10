package com.studhub.dataserver.model.entity;

import jakarta.persistence.*;

import java.time.OffsetDateTime;

@Entity
@Table(name = "bricklink_inventory")
public class BricklinkInventory {
    @Id
    @Column(name = "id", nullable = false)
    private Integer id;

    @Column(name = "inventory_data", columnDefinition = "jsonb")
    private String inventoryData;

    @Column(name = "updated_at", columnDefinition = "timestamptz")
    private OffsetDateTime updatedAt;
    
    @ManyToOne(fetch = FetchType.LAZY)
    @JoinColumn(name = "user_id", nullable = false)
    private Stud user;

    public Integer getInventoryId() {
        return id;
    }

    public void setInventoryId(Integer id) {
        this.id = id;
    }

    public Stud getUser() {
        return user;
    }

    public void setUser(Stud user) {
        this.user = user;
    }

    public String getInventoryData() {
        return inventoryData;
    }

    public void setInventoryData(String inventoryData) {
        this.inventoryData = inventoryData;
    }
}
