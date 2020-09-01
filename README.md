# GitHub Model Repo 

This repository is intended to host the models available in the Public Model Repo, it will become the master (single source of truth) for public models. 

## Folder Structure 

The repository will enforce that all models use the same folder convention:
The repository enforces the following structure. 

    \---models 
        | 
        +---com-example-temperaturecontroller 
        |       tc1.json 
        |       tc2.json 
        |       model-manifest.json
        | 
        \---com-example-thermostat 
        |       thermostat1.json 
        |       model-manifest.json

There will be one json file per interface version. File names must be lowercase but there is no enforcement on the file name. 

## DTMI Case Sensitive Rules 

Although the DTMI spec declares the @id as case sensitive, the folder names will use the lower-case version of the id replacing “:” with “-” without the “dtmi” prefix 

## File Names and Model Versions 

Model versions can be stored with any name, but the IDs, including the versions, must be unique in the folder. 

## DTMI ownership 

Each DTMI submitted will be associated to the first user who submitted the model, subsequent version updates must come from the same user. 

Ownership can be changed by submitting a PR to the global model-index.json file. 

## Model manifest

Each folder should contain a ```model-manifest.json``` file containing metadata about the modelsand versions in the current folder.

```json
[
    {
        "dtmi": "dtmi:com:example:TemperatureController;1",
        "path": "tc1.json",
        "owner": "ridomin",
        "depends": [
            "dtmi:com:example:Thermostat;1",
            "dtmi:azure:DeviceManagement:DeviceInformation;1"
        ]
    },
    {
        "dtmi": "dtmi:com:example:TemperatureController;2",
        "path": "tc2.json",
        "owner": "ridomin",
        "depends": [
            "dtmi:com:example:Thermostat;1",
            "dtmi:azure:DeviceManagement:DeviceInformation;1"
        ]
    }
]
```
| Index Field | Description |
| ----------- | ----------- |
| repoName | Must match the GitHub repo name |
| lastUpdate | dateTime with the last fileUpdate |
| lastCommit | lastCommit to master that produced the lastst update |
| Models | An array of | 
| +dtmi | Interface id in dtmi form, must be unique. Can be case sensitive |
| +path | Relative path to the file with the interface contents |
| -depends | An array of dependent (by extend, or component schema) interfaces if any. 

<br>

## Validation Process 

New model submissions will use Pull Requests from forked repos.  

The PR will include the interface files following the folder structure defined above, plus the additions to the model index. 

1. The PR will trigger the next validations. 

1. Folder names matches the lowercase DTMI for each interface.  

    1. The models are validated using the DTDL-Validation CLI (dotnet tool using the official parser) 

    1. Models will resolve external interfaces using the public repository. 

    1. DTMIs must be unique in the current public model repo. 

    1. DTMIs can only be updated (next versions) per the same user who submitted the first version 

    1. Inline models are not allowed 

    1. Model Updates are not allowed, it will require to rev the version. 

1. Policheck will find non-allowed terms per the Microsoft policy 

1. model-index.json will be updated with one entry per DTMI 

1. Submit all interfaces to the public model repo 

## Validation Status 

When the validation fails the PR will be rejected. Warnings might require a manual approval. 

# Public Repo Submission 

If the PR passes all validation checks without any error or warning, the model – with all required files- will be pushed to the public repository using the REST API. 

The submission process will take care of the interface dependencies. 