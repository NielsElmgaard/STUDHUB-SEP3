import pandas as pd

df = pd.read_csv('../data-server/data/parts_mapping.csv')

print(f"Total records before: {len(df)}")

df = df.drop_duplicates(keep='first')

print(f"Total records after: {len(df)}")

df.to_csv('../data-server/data/parts_mapping_clean.csv', index=False)