namespace RapidTransit.Core.Caching
{
    public interface ICacheControl<in TReload, TReloaded>
    {
        void Reload(TReload reload, out TReloaded reloaded);
    }


    public interface ICacheControl<in TReload, TReloaded, in TUpdate, TUpdated> :
        ICacheControl<TReload, TReloaded>
    {
        void Update(TUpdate update, out TUpdated updated);
    }
}