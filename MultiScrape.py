from bs4 import BeautifulSoup
import requests
import csv

root = 'https://www.klavkarr.com'
website = f'{root}/data-trouble-code-obd2.php'
result = requests.get(website)
content = result.text
soup = BeautifulSoup(content, 'lxml')

box = soup.find('div', class_='article-complementaire')

links = []

for link in box.find_all('a', href=True):
    links.append(link['href'])

print(links)

with open("editors.csv", "wt+", newline="", encoding='utf8') as f:
    writer = csv.writer(f, delimiter=';')
    unique_rows = set()

    for link in links:
        result = requests.get(f'{root}/{link}')
        content = result.text
        soup = BeautifulSoup(content, 'lxml')

        box = soup.find('div', class_='main_article-blog')

        title = box.find('table')

        # Supponendo che ci sia una lista di tabelle
        all_tables = soup.findAll("table")

        for table in all_tables:
            for row in table.findAll("tr"):
                csv_row = [cell.get_text() for cell in row.findAll(["td", "th"])]
                unique_row_key = hash(tuple(csv_row))
                if unique_row_key not in unique_rows:
                    writer.writerow(csv_row)
                    unique_rows.add(unique_row_key)


