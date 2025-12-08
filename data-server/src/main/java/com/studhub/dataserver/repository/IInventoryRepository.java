package com.studhub.dataserver.repository;

public interface IInventoryRepository {

    void upsertInventory(Integer id, String json, Integer userId);
}
