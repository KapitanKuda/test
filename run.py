from flask import Flask, jsonify, request
import sqlite3


app = Flask(__name__)


@app.route('/take', methods=['GET'])
def get_tasks():
    conn = sqlite3.connect('app.sqlite3')
    cursor = conn.cursor()
    cursor.execute("SELECT value FROM Val WHERE key=1")
    results = cursor.fetchall()
    value = [
        {
            'value': results[0][0]
        }
    ]
    conn.close()
    return jsonify(value)


@app.route('/add', methods=['POST'])
def add():
    data = request.args['name']

    data = int(data)+1
    new_value = [
        {
            'value': data
        }
    ]
    return jsonify( new_value)


if __name__ == '__main__':
    app.run(debug=True)