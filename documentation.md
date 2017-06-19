# Log
The log is where the history of all changes are saved. It consists of commits and changes. 

## Commits
Commits are saved in a DAG where each commit includes the hash of the one or many commits that directly precede it. Each commit is timestamped and contains a reference to the user that made the commit. It also contains a list of references to the changes that were made in the commit. It has the hash of itself as id.

## Changes
A change contains a reference to a model, an item and a property. It also contains the old value and the new value. It has the hash of itself as id.

# State
The current state of the system is described by models, properties, users and items. The users and items are the traditional mutable data in, for instance, a database. Models and Properties are metadata that describes the storage format of the users and items. A change in the storage format, for example that a property is added to a model, is recorded in a commit in the same way that a value change in an item is. In this way both the structure of the data and the data itself is recorded in the same way and can be played back.

## Models
The Models describe the structure of an item. In a database this would define the format of a table. It contains the name and a list of references to properties.

## Properties
Properties describe the format of a single value. In a database this would be a column. A property contains the name and datatype of the value.

## Users


## Items 