package com.studhub.dataserver.repository;

import com.studhub.dataserver.model.entity.BrickLinkOrder;
import jakarta.transaction.Transactional;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

@Repository
public interface BrickLinkOrderRepository extends JpaRepository<BrickLinkOrder, Integer>, IOrderRepository {

    @Transactional
    @Query(value = """
            INSERT INTO bricklink_order (id, order_data, user_id, updated_at)
            VALUES (:id, CAST(:data AS JSONB), :userId, now())
            ON CONFLICT (id)
            DO UPDATE SET order_data        = EXCLUDED.order_data,
                          user_id           = EXCLUDED.user_id,
                          updated_at        = now()
            WHERE bricklink_order.order_data IS DISTINCT FROM EXCLUDED.order_data
            RETURNING id
            """, nativeQuery = true)
    Integer upsertOrder(@Param("id") Integer id,
                        @Param("data") String json,
                        @Param("userId") Integer userId);
}
