from flask import Flask, request, jsonify
from flask_cors import CORS
import os
import hashlib
from urllib.parse import urlparse

app = Flask(__name__)
CORS(app)

SAVE_DIR = "saved_pages"
os.makedirs(SAVE_DIR, exist_ok=True)

def shorten_filename(url: str) -> str:
    parsed_url = urlparse(url)
    domain = parsed_url.netloc.replace('.', '_')
    path_hash = hashlib.md5(url.encode('utf-8')).hexdigest()
    return f"{path_hash}_{domain}.html"

@app.route("/save", methods=["POST"])
def save_html():
    data = request.get_json()
    html = data.get("html", "")
    url = data.get("url", "")
    
    if not html or not url:
        return jsonify({"error": "Missing HTML or URL"}), 400

    filename = os.path.join(SAVE_DIR, shorten_filename(url))
    
    try:
        with open(filename, "w", encoding="utf-8") as f:
            f.write(html)
        return jsonify({"message": f"Saved to {filename}"}), 200
    except Exception as e:
        return jsonify({"error": str(e)}), 500

if __name__ == "__main__":
    app.run(debug=True)
