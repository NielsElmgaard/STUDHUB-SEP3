package com.studhub.dataserver;

import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository
public interface StudRepository extends JpaRepository<Stud, String> {
    Optional<Stud> findByEmail(String email);
}
