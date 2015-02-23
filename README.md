# Modl
Modl is a Model framework that fits in nicely as the M in MVC frameworks.
It's written in C# and has the following core features:

* Static state
* Object cache
* Change tracking with versioning
* Readers and writers are plugins
* Keeps track of relations between models, including inheritance
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
* Combining data from various sources by relationsships
* Encryption
* Etc.

##Basic usage
Class Car implements empty interface IModl.

    Car car = new Car().Modl();
    Car car = Modl<Car>.New();

    Car car = Modl<Car>.Get(id);
    IEnumerable<Car> cars = Model<Car>.GetWhere(query);
    IEnumerable<Car> cars = Model<Car>.GetAll();

    bool carExists = Model<Car>.Any(id);
    bool carExists = Model<Car>.Any(query);

    car.Save(withRelations = true);
    car.Delete(withRelations = true);

##Current status
Pre alpha, only some things are working.

## Requirements
* .NET 4.5

## License
Released under the [GNU Lesser General Public License v3.0](http://www.gnu.org/licenses/lgpl.html).