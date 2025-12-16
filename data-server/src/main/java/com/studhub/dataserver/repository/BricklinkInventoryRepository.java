package com.studhub.dataserver.repository;

import com.studhub.dataserver.model.entity.BricklinkInventory;
import com.studhub.dataserver.repository.projection.BrickOwlDiffProjection;
import jakarta.transaction.Transactional;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.jpa.repository.Query;
import org.springframework.data.repository.query.Param;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface BricklinkInventoryRepository extends JpaRepository<BricklinkInventory, Integer>, IInventoryRepository {

    @Transactional
    @Query(value = """
            INSERT INTO bricklink_inventory (id, inventory_data, user_id, updated_at)
            VALUES (:id, CAST(:data AS JSONB), :userId, now())
            ON CONFLICT (id)
            DO UPDATE SET inventory_data = EXCLUDED.inventory_data,
                          user_id        = EXCLUDED.user_id,
                          updated_at     = EXCLUDED.updated_at
            WHERE bricklink_inventory.inventory_data IS DISTINCT FROM EXCLUDED.inventory_data
            RETURNING id
            """, nativeQuery = true)
    void upsertInventory(@Param("id") Integer id,
                         @Param("data") String json,
                         @Param("userId") Integer userId);

    /**
     * Compare and fetch inventories, that BrickOwl does not match with BrickLink
     * note:
     * 1. Only find PART due to lack of other (e.g. mini figure) id mapping between
     * BrickLink and BrickOwl
     * 2. Do not consider color for now also due to lack of color code mapping.
     * 3. split boid by - due to lack of color id. boid: itemid-colorid
     * TODO: remove limitation on result length after testing
     */
    @Transactional
    @Query(value = """
            WITH
            bo_inv AS (
                SELECT boi.id AS lot_id,
                       split_part(boi.inventory_data #>> '{boid}', '-', 1) AS boid,
                       boi.inventory_data #>> '{qty}' AS bo_quantity
                FROM brickowl_inventory AS boi
            ),
            bi_inv_with_map AS (
                SELECT bli.id AS bl_id,
                       bli.inventory_data #>> '{item,type}' AS type,
                       bli.inventory_data #>> '{quantity}' AS quantity,
                       bli.inventory_data #>> '{unit_price}' AS price,
                       bli.inventory_data #>> '{color_id}' AS color_id,
                       pm.brickowl_id AS boid
                FROM bricklink_inventory AS bli
                LEFT JOIN parts_map AS pm
                  ON pm.bricklink_id = bli.inventory_data #>> '{item,no}'
                WHERE bli.inventory_data #>> '{item,type}' = 'PART'
                  AND (bli.inventory_data #>> '{quantity}')::numeric > 0
                  AND pm.brickowl_id IS NOT NULL
            )
            SELECT
                bi.type,
                bi.quantity,
                bi.price,
                bi.boid,
                bo.lot_id,
                CASE
                    WHEN bo.lot_id IS NULL THEN 'CREATE'
                    WHEN bo.bo_quantity::numeric <> bi.quantity::numeric THEN 'UPDATE'
                    ELSE NULL
                END AS action
            FROM bi_inv_with_map AS bi
            LEFT JOIN bo_inv AS bo
              ON bi.boid = bo.boid
            WHERE
                bo.lot_id::int IS NULL
                OR bo.bo_quantity::numeric <> bi.quantity::numeric
            LIMIT 100000;
            """, nativeQuery = true)
    List<BrickOwlDiffProjection> findAllForBrickOwl(@Param("userId") Integer userId);


    List<BricklinkInventory> findAllByUserId(Integer studUserID);

}