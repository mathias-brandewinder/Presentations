// Core model

namespace HomeDepot

module Core = 

    type Observation = {
        Search:string
        Product:string }

    type Relevance = float 
    
    type Predictor = Observation -> Relevance

    type Feature = Observation -> float

    type Example = Relevance * Observation

    type Model = Feature []

    type Learning = Model -> Example [] -> Predictor
