package com.studhub.dataserver.model.entity;

import jakarta.persistence.Column;
import jakarta.persistence.Embeddable;

import java.io.Serializable;

@Embeddable
public class BlBoId implements Serializable {
    @Column
    private int bl_id;
    @Column
    private int bo_id;

    public int getBl_id() {
        return bl_id;
    }

    public void setBl_id(int bl_id) {
        this.bl_id = bl_id;
    }

    public int getBo_id() {
        return bo_id;
    }

    public void setBo_id(int bo_id) {
        this.bo_id = bo_id;
    }

}