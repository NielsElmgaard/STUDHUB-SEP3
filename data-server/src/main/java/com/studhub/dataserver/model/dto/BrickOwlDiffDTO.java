package com.studhub.dataserver.model.dto;

public class BrickOwlDiffDTO {
    private String type;
    private String qty;
    private String price;
    private String boid;
    private Integer lotId;
    private String action;

    public BrickOwlDiffDTO(String type, String qty, String price, String boid, Integer lotId, String action) {
        this.type = type;
        this.qty = qty;
        this.price = price;
        this.boid = boid;
        this.lotId = lotId;
        this.action = action;
    }

    public String getType() {
        return type;
    }

    public void setType(String type) {
        this.type = type;
    }

    public String getQty() {
        return qty;
    }

    public void setQty(String qty) {
        this.qty = qty;
    }

    public String getPrice() {
        return price;
    }

    public void setPrice(String price) {
        this.price = price;
    }

    public String getBoid() {
        return boid;
    }

    public void setBoid(String boid) {
        this.boid = boid;
    }

    public Integer getLotId() {
        return lotId;
    }

    public void setLotId(Integer lotId) {
        this.lotId = lotId;
    }

    public String getAction() {
        return action;
    }

    public void setAction(String action) {
        this.action = action;
    }
}
