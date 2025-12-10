package com.studhub.dataserver.config;

import javax.crypto.Cipher;
import javax.crypto.spec.GCMParameterSpec;
import javax.crypto.spec.SecretKeySpec;
import java.util.Base64;

public class ManualDecrypter {
  private static final String ALGORITHM = "AES/GCM/NoPadding";
  private static final int GCM_TAG_LENGTH = 128; // In bits
  private static final int IV_LENGTH = 12; // In bytes

  private static final String ENCRYPTED_DATA = "6t33fe5xXOFL52OnTaIHkVMB//6HMH370ve/koYqYTaTFy8jbwS/HC7Ofdg1+lArziUQgCl7QWJwsyLz";
  private static final String SECRET_KEY = "MySecretKey12345";

  public static void main(String[] args) {
    try {
      String decryptedValue = decrypt(ENCRYPTED_DATA, SECRET_KEY);
      System.out.println("Decrypted Consumer Key: " + decryptedValue);
    } catch (Exception e) {
      System.err.println("Decryption Failed: " + e.getMessage());
      e.printStackTrace();
    }
  }

  public static String decrypt(String dbData, String secretKey) throws Exception {
    if (dbData == null || dbData.isEmpty()) {
      return dbData;
    }

    SecretKeySpec keySpec = new SecretKeySpec(secretKey.getBytes(), "AES");
    byte[] combined = Base64.getDecoder().decode(dbData);

    if (combined.length < IV_LENGTH) {
      throw new IllegalArgumentException("Data too short for IV.");
    }

    // 1. Split IV (First 12 bytes)
    byte[] iv = new byte[IV_LENGTH];
    System.arraycopy(combined, 0, iv, 0, IV_LENGTH);

    // 2. Split Ciphertext + Tag (Remaining bytes)
    byte[] encrypted = new byte[combined.length - IV_LENGTH];
    System.arraycopy(combined, IV_LENGTH, encrypted, 0, encrypted.length);

    // 3. Decrypt using GCM mode
    Cipher cipher = Cipher.getInstance(ALGORITHM);
    GCMParameterSpec spec = new GCMParameterSpec(GCM_TAG_LENGTH, iv);
    cipher.init(Cipher.DECRYPT_MODE, keySpec, spec);

    return new String(cipher.doFinal(encrypted));
  }
}
