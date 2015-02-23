# Modl
Modl is a Model framework that fits in nicely as the M in MVC frameworks.
It's written in C# and has the following core features:

* Static state
* Object cache
* Change tracking with versioning
* Readers and writers are plugins
* Models have a unique id, a timestamp and a checksum
* Keeps track of relations between models, including inheritance
* Uses interfaces and extension methods to have as clean model classes as possible


##Use cases
* Memory
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


##Basic usage:
Class Car implements empty interface IModl:

var car = Modl<Car>.New();
var car = new Car().Modl();

var car = Modl<Car>.Get(id);
var cars = Model<Car>.GetWhere(query);
var cars = Model<Car>.GetAll();

var carExists = Model<Car>.Any(id);
var carExists = Model<Car>.Any(query);

car.Save(withRelations = true);
car.Delete(withRelations = true);


## Requirements
* .NET 4.5


## License
Released under the [GNU Lesser General Public License v3.0](http://www.gnu.org/licenses/lgpl.html).