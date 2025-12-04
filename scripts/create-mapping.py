import os
import requests
import csv
from itertools import product
import time

# In terminal, run export REBRICKABLE_API_KEY='your_api_key_here' to set your API key
api_key = os.getenv('REBRICKABLE_API_KEY')
# Make the API call
headers = {
    'Accept': 'application/json',
    'Authorization': f'key {api_key}'
}
is_end = False
page = 1
page_size = 1000

with open('../data-server/data/parts_mapping.csv', 'w', newline='') as csvfile:
    writer = csv.writer(csvfile)
    writer.writerow(['bricklink_id', 'brickowl_id'])
    print("Start fetching parts from Rebrickable...")

    while not is_end:
        print(f"Fetching page {page}...")
        response = requests.get('https://rebrickable.com/api/v3/lego/parts', params={ 'page': page, 'page_size': page_size}, headers=headers)

        if response.status_code == 200:
            data = response.json()
            
            # Process each part in results
            for part in data.get('results', []):
                external_ids = part.get('external_ids', {})
                bricklink_ids = external_ids.get('BrickLink', [])
                brickowl_ids = external_ids.get('BrickOwl', [])
                
                # Create all unique pairs between BrickLink and BrickOwl IDs
                for bl_id, bo_id in product(bricklink_ids, brickowl_ids):
                    writer.writerow([bl_id, bo_id])
            
            # Check if there's a next page
            if data.get('next'):
                page += 1
            else:
                is_end = True
            # avoid hitting rate limits
            time.sleep(1)
        else:
            print(f"Error: {response.status_code}")
            print(response.text)

