package com.studhub.dataserver.model.mapper;

import com.google.protobuf.Struct;
import com.google.protobuf.util.JsonFormat;
import com.studhub.dataserver.model.dto.InventoryDTO;
import org.springframework.stereotype.Component;

@Component
public class InventoryMapper {

    public InventoryDTO fromStruct(Struct struct, Integer userId, Integer id) {
        try {
            String json = JsonFormat.printer().print(struct);

            return new InventoryDTO(id, json, userId);

        } catch (Exception e) {
            throw new RuntimeException("Failed to parse proto inventory struct", e);
        }
    }
}

