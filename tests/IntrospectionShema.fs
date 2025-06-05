module IntrospectionShema

open System
open Expecto
open Snowflaqe
open FSharp.Data.LiteralProviders

[<Literal>]
let firstSchema = TextFile<"./Introspection.json">.Text

[<Literal>]
let typesFileName = "Types.fs"

let query =
    """
    query getAssets (
      $propertyId: PropertyID!,
      $filter: ObjectListFilter,
      $first: Int,
      $after: String
    ) {
      construction {
        property(id: $propertyId) {
          assets(first: $first, after: $after, filter: $filter) {
            pageInfo {
              endCursor
              hasPreviousPage
            }
            edges {
                id
                name
                number
                category
                warrantyExpiresAt
                eTag
                location {
                  floor
                  room
                  area
                }
              }
            }
          }
        }
      }
    }
"""

let introspectionTests =
    testList
        "Introspection"
        [ test "Timestamptz is converted to DateTimeOffset" {
              let schema = Schema.parse firstSchema

              match schema with
              | Error error -> failwith error
              | Ok schema ->
                  let generated =
                      let normalizeEnumCases = true
                      let globalTypes = CodeGen.createGlobalTypes schema normalizeEnumCases
                      let ns = CodeGen.createNamespace [ "Test" ] globalTypes
                      let file = CodeGen.createFile typesFileName [ ns ]
                      CodeGen.formatAst file typesFileName

                  let expected =
                      """namespace rec Test

/// Console login provider
[<Fable.Core.StringEnum; RequireQualifiedAccess>]
type ConsoleLoginProvider =
    | [<CompiledName "GoogleWorkspace">] GoogleWorkspace
    | [<CompiledName "Microsoft365">] Microsoft365

/// The `Filter` scalar type represents a filter on one or more fields of an object in an object list. The filter is represented by a JSON object where the fields are the complemented by specific suffixes to represent a query.
type ObjectListFilter() =
    inherit obj()

type InputUser =
    { loginProvider: ConsoleLoginProvider
      displayName: Option<string>
      email: string
      additionalEmail: Option<string>
      phone: Option<string> }

type InputApplicationTenant =
    { id: Option<string>
      name: string
      loginProvider: ConsoleLoginProvider
      loginProviderTenantId: Option<string>
      allowedDomains: list<string> }

type PenthouseUnit =
    { id: string
      name: string
      referenceNumber: Option<string>
      communityAccount: Option<string>
      subDivisionAccount: Option<string>
      clubAccount: Option<string>
      penthouseUnitFloors: list<string>
      notes: string }

type InputBuildingUnit =
    { id: string
      name: string
      referenceNumber: Option<string>
      communityAccount: Option<string>
      subDivisionAccount: Option<string>
      clubAccount: Option<string>
      notes: string }

type InputPatchBuilding =
    { name: Option<string>
      legalName: Option<string>
      address: Option<InputPatchPropertyAddress>
      developer: Option<string>
      square: Option<float>
      inputUnitsCount: Option<int>
      vertilincUrl: Option<string>
      accountingSystemUrl: Option<string>
      cameraSystemUrl: Option<string>
      notes: Option<string> }

type InputPatchPropertyAddress =
    { street: Option<string>
      city: Option<string>
      state: Option<string>
      country: Option<string>
      zip: Option<string> }

type PatchOperationOptions =
    { eTag: Option<string>
      allowOverwrite: Option<bool>
      returnPatched: Option<bool> }

type PatchPenthouseUnit =
    { name: Option<string>
      referenceNumber: Option<string>
      communityAccount: Option<string>
      subDivisionAccount: Option<string>
      clubAccount: Option<string>
      notes: Option<string> }

type PatchBuildingUnit =
    { name: Option<string>
      referenceNumber: Option<string>
      communityAccount: Option<string>
      subDivisionAccount: Option<string>
      clubAccount: Option<string>
      notes: Option<string> }

type InputPatchComplex =
    { name: Option<string>
      legalName: Option<string>
      address: Option<InputPatchPropertyAddress>
      developer: Option<string>
      vertilincUrl: Option<string>
      accountingSystemUrl: Option<string>
      cameraSystemUrl: Option<string>
      notes: Option<string> }

type InputArea = { name: string; notes: string }

type InputBuildingFloor =
    { id: string
      displayName: Option<string>
      notes: string }

type InputConsoleUserRole = { id: Option<string>; name: string }

type InputAreaRoom =
    { id: string
      displayName: Option<string>
      notes: string }

type PatchArea =
    { name: Option<string>
      notes: Option<string> }

type PatchAreaRoom =
    { name: Option<string>
      notes: Option<string> }

type InputPatchBuildingFloor =
    { displayName: Option<string>
      notes: Option<string> }

type PatchConsoleUserRole = { name: Option<string> }

type InputBuilding =
    { id: Option<string>
      prefix: string
      name: string
      legalName: string
      address: InputPropertyAddress
      complexId: Option<string>
      square: float
      developerId: Option<string>
      inputUnitsCount: string
      vertilincUrl: Option<string>
      accountingSystemUrl: Option<string>
      cameraSystemUrl: Option<string>
      notes: Option<string> }

type InputPropertyAddress =
    { street: string
      city: string
      state: string
      country: string
      zip: string }

type InputComplex =
    { id: Option<string>
      prefix: string
      name: string
      legalName: string
      address: InputPropertyAddress
      developer: Option<string>
      vertilincUrl: Option<string>
      accountingSystemUrl: Option<string>
      cameraSystemUrl: Option<string>
      notes: Option<string> }

type InputPatchApplicationTenant =
    { name: Option<string>
      loginProvider: Option<ConsoleLoginProvider>
      allowedDomains: Option<list<string>>
      authorizedTenants: Option<list<string>>
      loginProviderTenantId: Option<string> }

type InputPatchUser =
    { displayName: Option<string>
      additionalEmail: Option<string>
      phone: Option<string> }

type InputPatchUserDefinition =
    { loginProvider: Option<ConsoleLoginProvider>
      displayName: Option<string>
      email: Option<string>
      additionalEmail: Option<string>
      phone: Option<string> }

type InputDomainAsset =
    { id: Option<string>
      name: string
      owner: InputDeliveryAddress
      category: Option<string>
      glCode: string
      invoiceNumber: Option<string>
      expiresAt: Option<string>
      supplierId: Option<string>
      accountId: Option<string>
      accountLogin: Option<string>
      accountPassword: Option<string>
      accountPin: Option<string>
      notes: Option<string> }

type InputDeliveryAddress =
    { line1: string
      line2: Option<string>
      city: string
      state: string
      country: string
      zip: string
      contactId: string
      name: string }

type InputAssetLocation =
    { floor: string
      area: Option<string>
      room: Option<string> }

type InputLeasedPhysicalAsset =
    { id: Option<string>
      name: string
      modelId: string
      category: Option<string>
      glCode: Option<string>
      invoiceNumber: Option<string>
      monthlyCost: Option<decimal>
      description: Option<string>
      imageUri: Option<string>
      supplierId: Option<string>
      ownerId: Option<string>
      appraisedValue: Option<decimal>
      color: Option<string>
      beaconsAndTags: list<string>
      currentInsuranceContract: Option<string>
      currentLeaseContract: Option<string>
      warrantyExpiresAt: string
      notes: Option<string> }

type InputMechanicalAsset =
    { id: Option<string>
      serialNumber: string
      category: Option<string>
      glCode: Option<string>
      invoiceNumber: Option<string>
      name: string
      modelId: string
      builtAt: Option<string>
      imageUri: Option<string>
      description: Option<string>
      warrantyExpiresAt: Option<string>
      installedAt: Option<string>
      inServiceSince: Option<string>
      expectedEndOfLifeAt: Option<string>
      ipAddress: Option<string>
      beaconsAndTags: list<string>
      communicationProtocols: list<string>
      supplierId: Option<string>
      notes: Option<string> }

type InputModel =
    { manufacturer: string
      id: Option<string>
      name: string
      glCode: Option<string>
      description: Option<string>
      connectors: list<string>
      communicationProtocols: list<string> }

type InputOwnedPhysicalAsset =
    { id: Option<string>
      name: string
      modelId: string
      category: Option<string>
      glCode: Option<string>
      invoiceNumber: Option<string>
      description: Option<string>
      imageUri: Option<string>
      supplierId: Option<string>
      appraisedValue: Option<decimal>
      color: Option<string>
      beaconsAndTags: list<string>
      currentInsuranceContractId: Option<string>
      warrantyExpiresAt: string
      notes: Option<string> }

type InputSoftwareLicenseAsset =
    { id: Option<string>
      name: string
      licenseNumber: string
      category: Option<string>
      glCode: Option<string>
      invoiceNumber: Option<string>
      supplierId: Option<string>
      expiresAt: Option<string>
      description: Option<string>
      notes: Option<string> }

type InputWebsiteAsset =
    { id: Option<string>
      webAddressUri: string
      websiteVendorId: Option<string>
      category: Option<string>
      glCode: string
      invoiceNumber: Option<string>
      hostingSupplierId: Option<string>
      hostingLogin: Option<string>
      hostingPassword: Option<string>
      adminPanelAddressUri: Option<string>
      adminLogin: Option<string>
      adminPassword: Option<string>
      notes: Option<string> }

type PatchDomainAsset =
    { name: Option<string>
      expiresAt: Option<string>
      supplierId: Option<string>
      category: Option<string>
      accountId: Option<string>
      accountLogin: Option<string>
      accountPassword: Option<string>
      accountPin: Option<string>
      owner: Option<PatchDeliveryAddress>
      invoiceNumber: Option<string>
      glCode: Option<string>
      notes: Option<string> }

type PatchDeliveryAddress =
    { line1: Option<string>
      line2: Option<string>
      city: Option<string>
      state: Option<string>
      country: Option<string>
      zip: Option<string>
      contactId: Option<string>
      name: Option<string> }

type PatchLeasedPhysicalAsset =
    { name: Option<string>
      modelId: Option<string>
      glCode: Option<string>
      description: Option<string>
      category: Option<string>
      appraisedValue: Option<decimal>
      supplierId: Option<string>
      color: Option<string>
      beaconsAndTags: Option<list<string>>
      warrantyExpiresAt: Option<string>
      currentInsuranceContractId: Option<string>
      currentLeaseContractId: Option<string>
      invoiceNumber: Option<string>
      notes: Option<string>
      owner: Option<PatchDeliveryAddress>
      monthlyCost: Option<decimal> }

type PatchAssetLocation =
    { floor: Option<string>
      area: Option<string>
      room: Option<string> }

type PatchMechanicalAsset =
    { name: Option<string>
      modelId: Option<string>
      serialNumber: Option<string>
      builtAt: Option<string>
      glCode: Option<string>
      description: Option<string>
      category: Option<string>
      supplierId: Option<string>
      inServiceSince: Option<string>
      warrantyExpiresAt: Option<string>
      installedAt: Option<string>
      expectedEndOfLifeAt: Option<string>
      ipAddress: Option<string>
      beaconsAndTags: Option<list<string>>
      communicationProtocols: Option<list<string>>
      notes: Option<string> }

type PatchOwnedPhysicalAsset =
    { name: Option<string>
      modelId: Option<string>
      glCode: Option<string>
      description: Option<string>
      category: Option<string>
      appraisedValue: Option<decimal>
      supplierId: Option<string>
      color: Option<string>
      beaconsAndTags: Option<list<string>>
      warrantyExpiresAt: Option<string>
      currentInsuranceContractId: Option<string>
      invoiceNumber: Option<string>
      notes: Option<string> }

type PatchSoftwareLicenseAsset =
    { name: Option<string>
      licenseNumber: Option<string>
      supplierId: Option<string>
      category: Option<string>
      expiresAt: Option<string>
      description: Option<string>
      invoiceNumber: Option<string>
      glCode: Option<string>
      notes: Option<string> }

type PatchWebsiteAsset =
    { webAddressUri: Option<string>
      hostingSupplierId: Option<string>
      hostingLogin: Option<string>
      hostingPassword: Option<string>
      category: Option<string>
      adminPanelAddressUri: Option<string>
      adminLogin: Option<string>
      adminPassword: Option<string>
      invoiceNumber: Option<string>
      glCode: Option<string>
      notes: Option<string>
      supplierId: Option<string> }

"""

                  let trimmedGenerated = Utilities.trimContentEnd generated
                  let trimmedExpected = Utilities.trimContentEnd expected

                  Expect.equal trimmedGenerated trimmedExpected "The code is generated correctly"
          }

          ]
