using System.Collections;
using Il2CppInterop.Runtime.InteropTypes;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace LimesFashionPods.Utilities;

public static class AssetUtilities
{
    public static bool IsHandleSuccess(AsyncOperationHandle handle)
    {
        return handle.IsValid() &&
               handle is { Status: AsyncOperationStatus.Succeeded, Result: not null };
    }

    public static IEnumerator HandleAsynchronousAddressableOperation<T>(this AsyncOperationHandle<T> handle)
        where T : Il2CppObjectBase
    {
        if (!handle.IsDone) yield return handle;

        if (!IsHandleSuccess(handle))
        {
            Entrypoint.Logger.Msg(
                ConsoleColor.DarkRed,
                $"Failed to perform action in asynchronous Addressable handle! | OperationException: {(handle.IsValid() ? handle.OperationException.ToString() : "INVALID HANDLE!")} | Result == null: {!handle.IsValid() || handle.Result == null}");
            if (handle.IsValid()) handle.Release();

            yield break;
        }

        var res = handle.Result;

        var obj = res.TryCast<Object>();
        if (obj != null) obj.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
    }
}