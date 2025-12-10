package com.studhub.dataserver.model.entity;

import jakarta.persistence.*;

@Entity
@Table(
        name = "user_role",
        uniqueConstraints = @UniqueConstraint(columnNames = {"bl_id", "bo_id"})
)
public class BlBoInventoryMap {

    @EmbeddedId
    private BlBoId id;
    @ManyToOne
    @MapsId("bl_id")
    @JoinColumn(name = "bl_id")
    private BricklinkInventory bl_id;
    
    @ManyToOne
    @MapsId("bo_id")
    @JoinColumn(name = "bo_id")
    private BrickowlInventory bo_id;

    public BlBoId getId() {
        return id;
    }

    public void setId(BlBoId id) {
        this.id = id;
    }

    public BricklinkInventory getBl_id() {
        return bl_id;
    }

    public void setBl_id(BricklinkInventory bl_id) {
        this.bl_id = bl_id;
    }

    public BrickowlInventory getBo_id() {
        return bo_id;
    }

    public void setBo_id(BrickowlInventory bo_id) {
        this.bo_id = bo_id;
    }
}