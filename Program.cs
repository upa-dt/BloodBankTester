// See https://aka.ms/new-console-template for more information
using Newtonsoft.Json;
using UPA_EXTERNAL_MODELS;
using UPA_EXTERNAL_MODELS.Models.BloodBanks;
using UPA_EXTERNAL_MODELS.Models.Configs;
using UPA_SDK;

internal class Program
{
    private static void Main(string[] args)
    {
        /// You Need to add reference to UPA_EXTERNAL_MODELS project published at [https://github.com/upa-dt/UPA_EXTERNAL_MODELS]
        /// You Need to add reference to UPA_SDK project published at [https://github.com/upa-dt/UPA_SDK] 
        /// Add YOUR Provided Authentication Data to file "config.json" and this project will be functional
        /// Current Values In the config.json are Sample Authentication Data and will not Work
        LocalConfig conf = JsonConvert.DeserializeObject<LocalConfig>(File.ReadAllText("config.json"));

        BloodBank _bloodBankObject = new BloodBank(conf.API_ROOT_URL,
                                 conf.API_KEY,
                                 conf.API_USER,
                                 conf.API_PASSWORD,
                                 conf.API_OTP_SECRET,
                                 conf.API_SECRET,
                                 15);
        //Login to the API server
        var reso = _bloodBankObject.Login();
        if (!reso)// Failure Login
        {
            Console.WriteLine("FAILED Login");
            return;
        }
        Console.WriteLine("Successfull Login");

        ///Get List of Blood Groups
        var bloodGroups = _bloodBankObject.GetBloodGroups();
        Console.WriteLine("*************Blood Groups*************");
        Console.WriteLine(bloodGroups.SerializeObject());

        ///Get List of Blood Rh
        var bloodRh = _bloodBankObject.GetBloodRh();
        Console.WriteLine("*************Blood Rh*************");
        Console.WriteLine(bloodRh.SerializeObject());

        ///Get List of Professions
        var professions = _bloodBankObject.GetProfessions();
        Console.WriteLine("************* Professions *************");
        Console.WriteLine(professions.SerializeObject());


        ///Get List of BLood Form Types
        var bLoodFormType = _bloodBankObject.GetBLoodFormType();
        Console.WriteLine("************* BLood Form Types *************");
        Console.WriteLine(bLoodFormType.SerializeObject());


        ///Get List of Defer Types
        var deferTypes = _bloodBankObject.GetDeferTypes();
        Console.WriteLine("************* Defer Types *************");
        Console.WriteLine(deferTypes.SerializeObject());


        ///Get List of Countries
        var countries = _bloodBankObject.GetCountries();
        Console.WriteLine("************* Countries *************");
        Console.WriteLine(countries.SerializeObject());

        ///Get List of Cities
        var cities = _bloodBankObject.GetCities();
        Console.WriteLine("************* Cities *************");
        Console.WriteLine(cities.SerializeObject());

        ///Get List of Districts
        var districts = _bloodBankObject.GetDestricts();
        Console.WriteLine("************* Districts *************");
        Console.WriteLine(districts.SerializeObject());


        ///Register a new Donor
        var newDonorData = new RegisterDonorModel
        {
            firstName = "Mohamed",
            middleName = "Ibrahiem",
            thirdName = "Mahmoud",
            familyName = "Amer",
            address = "Maadi Cairo",
            birthDate = new DateTime(1980, 9, 1),
            countryId = 85,//[Egypt Code = 85] List of Countries is obtained previously by calling _bloodBankObject.GetCountries(); as explained
            cityId = 3223,//[Cairo Code = 3223] List of Cities is obtained previously by calling _bloodBankObject.GetCities(); as explained
            districtId = 31802,//[Cairo District Code = 31802] List of Districts is obtained previously by calling _bloodBankObject.GetDestricts(); as explained
                               // Country/City/District Codes should Match
            bloodGroup = 1,//[A Blood Group = 1] List of Blood Groups is obtained previously by calling _bloodBankObject.GetBloodGroups(); as explained
            bloodRh = 1,//[+ive Rh = 1] List of Blood Rh is obtained previously by calling _bloodBankObject.GetBloodRh(); as explained
            profession = 2,//[Officer Profession = 2] List of Professions is obtained previously by calling _bloodBankObject.GetProfessions(); as explained
            phone = "0100xxxxx38",//Donor Phone
            gender = GenderEnum.Male,
            idType = IDTypeEnum.NationalID,
            donerId = "2800901xxxxxx2",// This is Either National ID or Passport Number Depends on [idType] Field provided before
            comment = "This is A Test Donor"
        };
        var newDonorResult = _bloodBankObject.RegisterDonor(newDonorData);

        if (newDonorResult == null && newDonorResult.ErrorCode == 0)//Error Code != 0 Means There Is an Error
        {
            Console.WriteLine("Unable To Register New Donor");
            Console.WriteLine($"Regestration Error : {newDonorResult.ErrorMessage}");
            return;
        }
        int donorId = newDonorResult.Result;
        Console.WriteLine("************* New Donor Added  *************");
        Console.WriteLine($"  New Donor Added ID = {donorId}");
        Console.WriteLine("************* RegisterDonor Result  ********");
        Console.WriteLine(newDonorResult.SerializeObject());
        Console.WriteLine("********************************************");




        ///Register Donor Blood Information
        var newDonorBLoodInformation = new RegisterDonorBloodInfoModel
        {
            donorCentralId = donorId,// This Donor ID Obtained After Regestring the Donor (_bloodBankObject.RegisterDonor), His Central ID is Returned
            bloodGroup = 1,//[A Blood Group = 1] List of Blood Groups is obtained previously by calling _bloodBankObject.GetBloodGroups(); as explained
            bloodRh = 1,//[+ive Rh = 1] List of Blood Rh is obtained previously by calling _bloodBankObject.GetBloodRh(); as explained

        };
        var newDonorBloodInfoResult = _bloodBankObject.RegisterDonorBloodInfo(newDonorBLoodInformation);

        if (newDonorBloodInfoResult == null && newDonorBloodInfoResult.ErrorCode == 0)//Error Code != 0 Means There Is an Error
        {
            Console.WriteLine("Unable To Register Donor Blood Information");
            Console.WriteLine($"Regestration Error : {newDonorResult.ErrorMessage}");
            return;
        }
        int donorBloodInfoId = newDonorBloodInfoResult.Result;
        Console.WriteLine("************* Donor Blood Information Added  *************");
        Console.WriteLine($"  Donor Blood Info Added ID = {donorBloodInfoId}  /*Not Important Value, Just For Record*/");
        Console.WriteLine("************* Register Donor Blood Information Result  ********");
        Console.WriteLine(newDonorBloodInfoResult.SerializeObject());
        Console.WriteLine("********************************************");

        // Now i Will Search For a donor to Know His Information using His National ID, and ID Type
        var searchDonorRequest = new DonorSearchModel
        {
            donerId = "2800901xxxxxx2",// The Registered Donor in Previous Example
            idType = IDTypeEnum.NationalID
        };
        var findDonorResult = _bloodBankObject.GetDonor(searchDonorRequest);
        if (findDonorResult == null || findDonorResult.ErrorCode != 0)//Error Code != 0 Means There Is an Error
        {
            Console.WriteLine($"Donor Search Error : {findDonorResult.ErrorMessage}");
            return;
        }

        if (findDonorResult.Result.info == null || findDonorResult.Result.info.Count < 1)// Donor Not Found
        {
            Console.WriteLine("Donor Info Not Exist, Please Register Him as a new Donor");
            return;
        }
        Console.WriteLine("************* Donor Personal Info *************");
        Console.WriteLine(findDonorResult.Result.info.SerializeObject());
        Console.WriteLine("************* Donor Deferral Info *************");
        if (findDonorResult.Result.deferral == null || findDonorResult.Result.deferral.Count < 1)
            Console.WriteLine("No Deferrals For This Donor");
        else
            Console.WriteLine(findDonorResult.Result.deferral.SerializeObject());
        Console.WriteLine("************* Donor Blood Information *************");
        if (findDonorResult.Result.deferral == null || findDonorResult.Result.deferral.Count < 1)
            Console.WriteLine("No Blood Information This Donor");
        else
            Console.WriteLine(findDonorResult.Result.bloodInfo.SerializeObject());
        Console.WriteLine("********************************************");

        // Register new Deferral For a donor
        DeferralModel newDeferralModel = new DeferralModel
        {
            deferId = 1,//["CANCER/" = 1] List of Deferral is obtained previously by calling _bloodBankObject.GetDeferTypes(); as explained
            deferStartDate = DateTime.Now,
            deferEndDate = null,//Null Value Means forever as Cancer is Permenant Deferral, If Deferral is not pemenant then here should be the end date of deferral
            donationDate = null,//Null oR mention Here the last donation date of this donor
            donorCentralId = donorId,// ID Of Donor either obtained after regestring the donor [_bloodBankObject.RegisterDonor] or by searching for him[_bloodBankObject.GetDonor]
            comment = "Any Additional Deferral Comments to be mentioned here"
        };
        var regDeferralResult = _bloodBankObject.RegisterDefer(newDeferralModel);
        if (regDeferralResult == null || regDeferralResult.ErrorCode != 0)//Error Code != 0 Means There Is an Error
        {
            Console.WriteLine($"Register Deferral Error : {regDeferralResult.ErrorMessage}");
        }
        else
        {
            Console.WriteLine("************* Register Defer Result *************");
            Console.WriteLine(regDeferralResult.SerializeObject());
        }
        InventoryTransactionModel transaction = new InventoryTransactionModel
        {
            transactionType = BloodTransactionTypeEnum.SET_VALUE,//Type Of Transaction, Either Set the Current Qyantity to the provided bagQty or Add bagQty to the current inventory Quantity or Subtract
            inventoryType = BloodInventoryTypeEnum.FreeToUse,// This will determin the type of Inventory Based on BloodInventoryTypeEnum
            bagQty = 10,//Quantity Of Bags
            bloodFormType = 1, ////[Pure Blood = 1] List of Countries is obtained previously by calling _bloodBankObject.GetCountries() as explained
            bloodGroup = 1,//[A Blood Group = 1] List of Blood Groups is obtained previously by calling _bloodBankObject.GetBloodGroups(); as explained
            bloodRh = 1,//[+ive Rh = 1] List of Blood Rh is obtained previously by calling _bloodBankObject.GetBloodRh(); as explained
            expireDate = DateTime.Now.AddDays(90),// Expire Date of This Blood Bags Quantity             
        };
        var addInventoryTransactionResult = _bloodBankObject.AddInventoryTransaction(transaction);
        if (addInventoryTransactionResult == null || addInventoryTransactionResult.ErrorCode != 0)//Error Code != 0 Means There Is an Error
        {
            Console.WriteLine($"Add Inventory Transaction Error : {addInventoryTransactionResult.ErrorMessage}");
        }
        else
        {
            Console.WriteLine("************* Add Inventory Transaction Result *************");
            Console.WriteLine(addInventoryTransactionResult.SerializeObject());
        }
        /// To Search All Inventory then All Filter Parameter Should be Null, This will return the entire Inventory
        /// To Narrow the filter for specific filter value, the filter should be assigned to the corresponding Field
        InventorySearchModel searchInvFilter = new InventorySearchModel
        {
            inventoryType = null,//Means All Inventory Transaction Types
            bloodFormType = 1, ////TO Find All Blood Form Types then provide bloodFormType = null
                               ////[Pure Blood = 1] List of Countries is obtained previously by calling _bloodBankObject.GetCountries() as explained
            bloodGroup = 1,///TO Find All Blood Groups then provide bloodGroup = null
                           //[A Blood Group = 1] List of Blood Groups is obtained previously by calling _bloodBankObject.GetBloodGroups(); as explained
            bloodRh = 1,//TO Find All Blood Rh then provide bloodRh = null
                        //[+ive Rh = 1] List of Blood Rh is obtained previously by calling _bloodBankObject.GetBloodRh(); as explained
            fromExpireDate = null,/// Null Means All regardless from Exoire Date
                                  ///To Specify a starting Search date for Expire Date  then specify Here
            toExpireDate = null,/// Null Means All regardless from Exoire Date
                                ///To Specify a Ending Search date for Expire Date  then specify Here 
        };
        var _inventory = _bloodBankObject.SearchInventory(searchInvFilter);
        if (_inventory == null || _inventory.ErrorCode != 0)//Error Code != 0 Means There Is an Error
        {
            Console.WriteLine($"Search Inventory Transaction Error : {_inventory.ErrorMessage}");
        }
        else
        {
            Console.WriteLine("************* Search Inventory  Result *************");
            Console.WriteLine(_inventory.SerializeObject());
        }
        Console.WriteLine("************** THE END *********************");
        ///////////// END OF SAMPLE PROJECT //////////////////////////
        ///////////// For Support Please Call 15556 //////////////////
        ///  THANK YOU ///
    }

}