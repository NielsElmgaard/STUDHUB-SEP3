package com.studhub.dataserver.repository;

import com.studhub.dataserver.model.entity.BricklinkInventory;
import jakarta.transaction.Transactional;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Modifying;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

@Repository
public interface BricklinkInventoryRepository extends JpaRepository<BricklinkInventory, Integer> {

    @Modifying
    @Transactional
    @Query(value = """
            INSERT INTO bricklink_inventory (id, inventory_data, user_id)
            VALUES (:id, CAST(:data AS JSONB), :userId)
            ON CONFLICT (id)
            DO UPDATE SET inventory_data = EXCLUDED.inventory_data,
                          user_id        = EXCLUDED.user_id
            """, nativeQuery = true)
    void upsertInventory(@Param("id") Integer id,
                         @Param("data") String json,
                         @Param("userId") Integer userId);
}