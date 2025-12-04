package com.studhub.dataserver.model.dto;

public class InventoryDTO {
    Integer inventoryId;
    String json;
    Integer userId;

    // Constructor
    public InventoryDTO(Integer inventoryId, String json, Integer userId) {
        this.inventoryId = inventoryId;
        this.json = json;
        this.userId = userId;
    }

    // Getters and setters
    public Integer getInventoryId() {
        return inventoryId;
    }

    public void setInventoryId(Integer inventoryId) {
        this.inventoryId = inventoryId;
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
