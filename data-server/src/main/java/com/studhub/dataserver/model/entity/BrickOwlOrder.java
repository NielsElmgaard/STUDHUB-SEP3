package com.studhub.dataserver.model.entity;

import jakarta.persistence.*;
import org.hibernate.annotations.UpdateTimestamp;

import java.time.OffsetDateTime;

@Entity
@Table(name = "bricklink_order")
public class BrickOwlOrder {
    @Id
    @Column(name = "id", nullable = false)
    private Integer id;

    @Column(name = "order_data", columnDefinition = "jsonb")
    private String orderData;

    @UpdateTimestamp
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

    public String getOrderData() {
        return orderData;
    }

    public void setOrderData(String orderData) {
        this.orderData = orderData;
    }
}
