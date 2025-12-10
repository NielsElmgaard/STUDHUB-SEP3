package com.studhub.dataserver.repository;

import com.studhub.dataserver.model.entity.BlBoId;
import com.studhub.dataserver.model.entity.BlBoInventoryMap;
import org.springframework.data.jpa.repository.JpaRepository;

public interface BlBoInventoryMapRepository extends JpaRepository<BlBoInventoryMap, BlBoId> {
}
