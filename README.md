# Modl
Modl is a Model framework that fits in nicely as the M in MVC frameworks.

The basic idea is to have a single model with objects relating to each other that spans different storage formats.
For instance you can have some objects stored on disk in json, 
some in a database and yet others being fetched as xml over the network.
All writes are done as transactions.

The idea is also to save diffs of all edits in a merkle tree, 
so that you can go back in history to fetch data and see who edited which objects and when.

It's written in C# and has the following core features:

* Static state
* Object cache
* Change tracking with versioning
* Readers and writers are plugins
* Keeps track of relations between models
* Uses interfaces and extension methods to leave your classes as clean as possible

##Use cases
* Memory storage
* Json
* Xml
* Csv
* Multiple databases
* Binary files
* Compressed files
* Mvc model binding
* Easy unit testing
* Logging
* Combining data from various sources
* Encryption
* Etc.

##Basic usage
Some examples of usage.

    public class Car : IModl
    {
        public IModlData Modl { get; set; }
    }

    Car car = new Car();
    Car car = Modl<Car>.New();

    car.SetId(Guid.NewGuid());
    Guid id = car.GetId();

    bool isModified = car.IsModified();
    bool isNew = car.IsNew();
    bool isDeleted = car.IsDeleted();

    Car car = Modl<Car>.Get(id);
    IEnumerable<Car> cars = Model<Car>.GetWhere(query);
    IEnumerable<Car> cars = Model<Car>.GetAll();

    bool carExists = Model<Car>.Any(id);
    bool carExists = Model<Car>.Any(query);

    car.Save(withRelations = true);
    car.Delete();

##Current status
Pre alpha, only some things are working.

## Requirements
* C# 6
* .NET 4.5


## License
Released under the [The MIT License (MIT)](http://opensource.org/licenses/MIT).