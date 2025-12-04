package com.studhub.dataserver.model.entity;

import jakarta.persistence.*;

@Entity
public class BrickOwlConnection {
    @Id
    private int id; // PK
    private String apiKey;
    @OneToOne
    @MapsId
    @JoinColumn(name = "stud_id")
    private Stud stud; // FK

    public Stud getStud() {
        return stud;
    }

    public void setStud(Stud stud) {
        this.stud = stud;
    }

    public int getId() {
        return id;
    }

    // Should only be used for testing
    public void setId(int id) {
        this.id = id;
    }

    public String getApiKey() {
        return apiKey;
    }

    public void setApiKey(String apiKey) {
        this.apiKey = apiKey;
    }
}
