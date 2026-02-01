using ClinicDataAccess;
using System.Xml.Linq;

namespace ClininBusinissLayer
{



    public class Person
    {
        public enum enMode { AddNew,Update}
        public enMode Mode = enMode.AddNew;

        public int Id { get; set; }
        public string Name { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string Gendor { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        
        public PersonDTO PDTO
        {
            get
            {
                return new PersonDTO(this.Id, this.Name, this.DateOfBirth, this.Gendor,
            this.PhoneNumber, this.Email, this.Address);
            }
        }
        
        public Person(PersonDTO personDTO,enMode mode = enMode.AddNew)
        {
            this.Id = personDTO.Id;
            this.Name = personDTO.Name;
            this.DateOfBirth = personDTO.DateOfBirth;
            this.Gendor = personDTO.Gendor;
            this.PhoneNumber = personDTO.PhoneNumber;
            this.Email = personDTO.Email;
            this.Address = personDTO.Address;
            Mode = mode;
        }

        public static List<PersonDTO> GetAll()
        {
            return clsPersonsDataAccess.GetAllPersons();
        }

        public static Person? Find(int id )
        {
            PersonDTO? p = clsPersonsDataAccess.GetPersonByID(id);
            if(p != null)
            {
                return new Person(p,enMode.Update);
            }else
            {
                return null;
            }
        }


         bool _AddNewPerson()
        {
            this.Id = clsPersonsDataAccess.AddNewPerson(PDTO);
            return (this.Id != -1);
        }

        bool _UpdatePerson()
        {
            return clsPersonsDataAccess.UpdatePerson(PDTO);
        }

        public bool Save()
        {
            switch (Mode)
            {
                case enMode.AddNew:
                    return _AddNewPerson();

                case enMode.Update:
                    return _UpdatePerson();

                default:
                    return false;
            }
        }

        public static bool IsPersonExist(int id)
        {
            Person? p = Find(id);

            return (p != null);
        }

        public static bool Delete(int id)
        {
            return clsPersonsDataAccess.DeletePerson(id);
        }

    }
}
