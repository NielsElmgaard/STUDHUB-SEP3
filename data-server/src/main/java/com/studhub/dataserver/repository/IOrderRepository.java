package com.studhub.dataserver.repository;

public interface IOrderRepository {

    Integer upsertOrder(Integer id, String json, Integer userId);
}
