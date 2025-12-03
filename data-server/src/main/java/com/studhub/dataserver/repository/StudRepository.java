package com.studhub.dataserver.repository;

import com.studhub.dataserver.model.entity.Stud;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;

import java.util.Optional;

@Repository public interface StudRepository extends JpaRepository<Stud, String>
{
  Optional<Stud> findByEmail(String email);
  Optional<Stud> findById(Long id);

}
