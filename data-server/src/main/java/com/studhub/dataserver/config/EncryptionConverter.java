package com.studhub.dataserver.config;

import jakarta.persistence.AttributeConverter;
import jakarta.persistence.Converter;

import javax.crypto.Cipher;
import javax.crypto.spec.GCMParameterSpec;
import javax.crypto.spec.SecretKeySpec;
import java.nio.charset.StandardCharsets;
import java.security.SecureRandom;
import java.util.Base64;

// https://thorben-janssen.com/how-to-use-jpa-type-converter-to/
@Converter public class EncryptionConverter
    implements AttributeConverter<String, String>
{

  private static final String ALGORITHM = "AES/GCM/NoPadding";
  private static final int GCM_TAG_LENGTH = 128;
  private static final int IV_LENGTH = 12; // 12 bytes = 96 bits
  private static final String SECRET_KEY_ENV = "APP_SECRET_KEY";

  private final SecretKeySpec keySpec;

  public EncryptionConverter()
  {
    String key = System.getenv(SECRET_KEY_ENV);
    if (key == null || key.length() != 16)
    {
      throw new IllegalStateException(
          "Environment variable APP_SECRET_KEY must be 16 chars for AES-128");
    }
    this.keySpec = new SecretKeySpec(key.getBytes(StandardCharsets.UTF_8),
        "AES");
  }

  @Override public String convertToDatabaseColumn(String attribute)
  {
    if (attribute == null || attribute.isEmpty())
    {
      return attribute;
    }
    try
    {
      // Generate random IV
      byte[] iv = new byte[IV_LENGTH];
      SecureRandom random = new SecureRandom();
      random.nextBytes(iv);

      Cipher cipher = Cipher.getInstance(ALGORITHM);
      GCMParameterSpec spec = new GCMParameterSpec(GCM_TAG_LENGTH, iv);
      cipher.init(Cipher.ENCRYPT_MODE, keySpec, spec);
      byte[] encrypted = cipher.doFinal(attribute.getBytes());

      // Combine IV + ciphertext
      byte[] combined = new byte[iv.length + encrypted.length];
      System.arraycopy(iv, 0, combined, 0, iv.length);
      System.arraycopy(encrypted, 0, combined, iv.length, encrypted.length);

      return Base64.getEncoder().encodeToString(combined);
    }
    catch (Exception e)
    {
      throw new RuntimeException("Error encrypting attribute", e);
    }
  }

  @Override public String convertToEntityAttribute(String dbData)
  {
    if (dbData == null || dbData.isEmpty())
      return dbData;
    try
    {
      byte[] combined = Base64.getDecoder().decode(dbData);

      if (combined.length < IV_LENGTH)
      {
        throw new RuntimeException(
            "Corrupted encrypted data: length " + combined.length
                + " is shorter than required IV length " + IV_LENGTH);
      }

      byte[] iv = new byte[IV_LENGTH];
      byte[] encrypted = new byte[combined.length - IV_LENGTH];
      System.arraycopy(combined, 0, iv, 0, IV_LENGTH);
      System.arraycopy(combined, IV_LENGTH, encrypted, 0, encrypted.length);

      Cipher cipher = Cipher.getInstance(ALGORITHM);
      GCMParameterSpec spec = new GCMParameterSpec(GCM_TAG_LENGTH, iv);
      cipher.init(Cipher.DECRYPT_MODE, keySpec, spec);

      return new String(cipher.doFinal(encrypted));
    }
    catch (Exception e)
    {
      throw new RuntimeException(
          "Error decrypting attribute: " + e.getMessage(), e);
    }
  }
}
