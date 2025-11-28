from fastapi import FastAPI
from pydantic import BaseModel
from typing import List

app = FastAPI()

class MatchRequest(BaseModel):
    user_id: int
    projet_ids: List[int]

class MatchResponse(BaseModel):
    projet_id: int
    score: float

@app.get("/")
def root():
    return {"message": "Matching IA OK"}

@app.post("/recommend", response_model=List[MatchResponse])
def recommend(req: MatchRequest):
    # TODO : ici tu mettras ton vrai modèle IA plus tard
    return [
        MatchResponse(projet_id=p_id, score=0.8)
        for p_id in req.projet_ids
    ]
