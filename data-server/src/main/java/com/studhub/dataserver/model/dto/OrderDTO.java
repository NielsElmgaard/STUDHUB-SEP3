package com.studhub.dataserver.model.dto;

public class OrderDTO {
    Integer orderId;
    String json;
    Integer userId;

    public OrderDTO(Integer orderId, String json, Integer userId) {
        this.orderId = orderId;
        this.json = json;
        this.userId = userId;
    }

    public Integer getOrderId() {
        return orderId;
    }

    public void setOrderId(Integer orderId) {
        this.orderId = orderId;
    }

    public String getJson() {
        return json;
    }

    public void setJson(String json) {
        this.json = json;
    }

    public Integer getUserId() {
        return userId;
    }

    public void setUserId(Integer userId) {
        this.userId = userId;
    }
}
