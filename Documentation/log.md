# Log

The log is where the history of all changes are saved. It consists of commits and changes.

## Commits

Commits are saved in a DAG where each commit includes the hash of the one or many commits that directly precede it. Each commit is timestamped and contains a reference to the user that made the commit. It also contains a list of references to the changes that were made in the commit. It has the hash of itself as id.

    commit_hash: {
        "type": "commit",
        "time": "2017-06-26T21:34:00.692271Z",
        "user": user_hash,
        "changes": [
            change_hash,
            change_hash,
            ...
        ],
        "parents": [
            commit_hash,
            commit_hash,
            ...
        ]
    }

## Changes

A change contains a reference to a model, an item and a property. It also contains the old value and the new value. It has the hash of itself as id.

    change_hash: {
        "type": "change",
        "changed_type": type,
        "old_hash": hash,
        "new_hash": hash,
        "diff": diff_string
    }
