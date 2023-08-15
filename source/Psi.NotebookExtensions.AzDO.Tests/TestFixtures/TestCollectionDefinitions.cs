namespace Psi.NotebookExtensions.AzDO.Services.Tests;

[CollectionDefinition("AzDO Client Factory")]
public class AzDOClientFactoryTestCollection : ICollectionFixture<AzDOClientFactoryTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}

[CollectionDefinition("AzDO Git Client")]
public class GitHttpClientTestCollection : ICollectionFixture<GitHttpClientTestFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
