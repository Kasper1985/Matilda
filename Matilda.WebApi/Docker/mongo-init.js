db.auth("root", "password")

db.getSiblingDB("matilda");

db.createUser({
    user: "matilda",
    pwd: "matilda",
    roles: [
        {
            role: "readWrite",
            db: "matilda"
        }
    ]
})

db = new Mongo().getDB("matilda");

db.createCollection("Chat");
db.createCollection("ChatMessage");
