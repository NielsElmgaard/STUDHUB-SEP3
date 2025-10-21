package com.studhub.dataserver;

import jakarta.persistence.*;

@Entity
public class Stud {
    @Id
    @Column(nullable = false, unique = true)
    private String email; // Primary key

    private String username;
    private String passwordHash;

    public String getEmail() { return email; }
    public void setEmail(String email) { this.email = email; }

    public String getUsername() { return username; }
    public void setUsername(String username) { this.username = username; }

    public String getPasswordHash() { return passwordHash; }
    public void setPasswordHash(String passwordHash) { this.passwordHash = passwordHash; }
}

