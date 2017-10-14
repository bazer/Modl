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

## Use cases

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

## Basic usage

Some examples of usage.

    public interface ICar : IMutable
    {
        [Id(automatic: true)]
        Guid Id { get; }
        [Name("CarName")]
        string Name { get; set; }
        IManufacturer Manufacturer { get; set; }
    }

    public interface IManufacturer : IMutable
    {
        [Id]
        Guid ManufacturerID { get; }
        string Name { get; set; }
        IList<ICar> Cars { get; }
    }

    var car = M.New<ICar>();
    car.Name = "Model S";
    car.Manufacturer = M.New<IManufacturer>()
        .Change(x => x.ManufacturerID, Guid.NewGuid())
        .Change(x => x.Name, "Tesla");

    changes = car.GetChanges();
    var user = new User("user@example.com");
    var commit = changes.Commit(user);
    commit.Push();

    var loaded_car = M.Get<ICar>(car.Id);

    Assert.Equal("Model S", loaded_car.Name);
    Assert.Equal("Tesla", loaded_car.Manufacturer.Name);
    Assert.Equal("Model S", loaded_car.Manufacturer.Cars.Single(x => x.Id == loaded_car.Id).Name);

## Documentation

More comprehensive documentation can be found here.

[Documentation](Documentation/index.md)

## Current status

Pre alpha, only some things are working.

## Requirements

* .NET Standard 2.0

## License

Released under the [The MIT License (MIT)](http://opensource.org/licenses/MIT).