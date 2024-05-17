using AssaultCubeHack;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Framework
{
    internal static class Memory
    {

        //CROUCH
        public static volatile float _CROUCH = 74F;

        public static T Read<T>(ulong address) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            byte[] buffer = new byte[size];
            unsafe
            {
                fixed (byte* pBuffer = buffer)
                {

                    if (Offsets.vmm.MemRead(Offsets.processPid, address, (uint)size, (nint)pBuffer, Framework.Vmm.FLAG_NOCACHE | Framework.Vmm.FLAG_NOPAGING | Framework.Vmm.FLAG_ZEROPAD_ON_FAIL | Framework.Vmm.FLAG_NOPAGING_IO) == size)
                    {
                        return ByteArrayToStructure<T>(buffer);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to read {typeof(T).Name} from memory.");
                        return default(T);
                    }
                }
            }
        }

        public static T ReadScatter<T>(ulong address) where T : struct
        {
            int size = Marshal.SizeOf(typeof(T));
            unsafe
            {

                byte[] buffer = Offsets.vmmScatter.Read(address, (uint)size);
                return ByteArrayToStructure<T>(buffer);
            }
        }

        public static void addScatterReadRequest(ulong address, uint size)
        {
            Framework.Offsets.vmmScatter.Prepare(address, (uint)size);
        }

        public static void ExecuteReadScatter()
        {

            Framework.Offsets.vmmScatter.Execute();
        }

        public static string ReadStringScatter(ulong address, ulong size)
        {
            unsafe
            {

                byte[] buffer = Offsets.vmmScatter.Read(address, (uint)size);

                //encode bytes to ascii
                for (int i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i] == 0)
                    {
                        byte[] tmpBuffer = new byte[i];
                        Buffer.BlockCopy(buffer, 0, tmpBuffer, 0, i);
                        return Encoding.ASCII.GetString(tmpBuffer);
                    }
                }
                return Encoding.ASCII.GetString(buffer);
            }
        }

        public static string ReadString(ulong address, ulong size)
        {
            byte[] buffer = new byte[size];
            unsafe
            {
                fixed (byte* pBuffer = buffer)
                {
                    if (Offsets.vmm.MemRead(Offsets.processPid, address, (uint)size, (nint)pBuffer, Framework.Vmm.FLAG_NOCACHE | Framework.Vmm.FLAG_NOPAGING | Framework.Vmm.FLAG_ZEROPAD_ON_FAIL | Framework.Vmm.FLAG_NOPAGING_IO) == size)
                    {
                        //encode bytes to ascii
                        for (int i = 0; i < buffer.Length; i++)
                        {
                            if (buffer[i] == 0)
                            {
                                byte[] tmpBuffer = new byte[i];
                                Buffer.BlockCopy(buffer, 0, tmpBuffer, 0, i);
                                return Encoding.ASCII.GetString(tmpBuffer);
                            }
                        }
                        return Encoding.ASCII.GetString(buffer);
                    }
                    else
                    {
                        Console.WriteLine($"Failed to read memory.");
                        return default;
                    }
                }
            }
        }



        /// <summary>
        /// Reads 16 consecutive floats into a Matrix
        /// </summary>
        public static Matrix ReadMatrix(ulong address)
        {
            //float matrix[16]; 16-value array laid out contiguously in memory       
            byte[] buffer = new byte[16 * 4];
            int size = 64; //16 * 4 = 64
            unsafe
            {
                fixed (byte* pBuffer = buffer)
                {
                    if (Offsets.vmm.MemRead(Offsets.processPid, address, (uint)size, (nint)pBuffer, Framework.Vmm.FLAG_NOCACHE | Framework.Vmm.FLAG_NOPAGING | Framework.Vmm.FLAG_ZEROPAD_ON_FAIL | Framework.Vmm.FLAG_NOPAGING_IO) == size)
                    {
                        //convert bytes to floats
                        Matrix mat = new Matrix();
                        mat.m11 = BitConverter.ToSingle(buffer, (0 * 4));
                        mat.m12 = BitConverter.ToSingle(buffer, (1 * 4));
                        mat.m13 = BitConverter.ToSingle(buffer, (2 * 4));
                        mat.m14 = BitConverter.ToSingle(buffer, (3 * 4));

                        mat.m21 = BitConverter.ToSingle(buffer, (4 * 4));
                        mat.m22 = BitConverter.ToSingle(buffer, (5 * 4));
                        mat.m23 = BitConverter.ToSingle(buffer, (6 * 4));
                        mat.m24 = BitConverter.ToSingle(buffer, (7 * 4));

                        mat.m31 = BitConverter.ToSingle(buffer, (8 * 4));
                        mat.m32 = BitConverter.ToSingle(buffer, (9 * 4));
                        mat.m33 = BitConverter.ToSingle(buffer, (10 * 4));
                        mat.m34 = BitConverter.ToSingle(buffer, (11 * 4));

                        mat.m41 = BitConverter.ToSingle(buffer, (12 * 4));
                        mat.m42 = BitConverter.ToSingle(buffer, (13 * 4));
                        mat.m43 = BitConverter.ToSingle(buffer, (14 * 4));
                        mat.m44 = BitConverter.ToSingle(buffer, (15 * 4));
                        return mat;
                    }
                    else
                    {
                        Console.WriteLine("Failed to read memory!");
                        return default;
                    }
                }
            }
        }

        public static Matrix ReadMatrixScatter(ulong address)
        {
            unsafe
            {
                byte[] buffer = Offsets.vmmScatter.Read(address, 64);

                //convert bytes to floats
                Matrix mat = new Matrix();
                mat.m11 = BitConverter.ToSingle(buffer, (0 * 4));

                mat.m12 = BitConverter.ToSingle(buffer, (1 * 4));
                mat.m13 = BitConverter.ToSingle(buffer, (2 * 4));
                mat.m14 = BitConverter.ToSingle(buffer, (3 * 4));

                mat.m21 = BitConverter.ToSingle(buffer, (4 * 4));
                mat.m22 = BitConverter.ToSingle(buffer, (5 * 4));
                mat.m23 = BitConverter.ToSingle(buffer, (6 * 4));
                mat.m24 = BitConverter.ToSingle(buffer, (7 * 4));

                mat.m31 = BitConverter.ToSingle(buffer, (8 * 4));
                mat.m32 = BitConverter.ToSingle(buffer, (9 * 4));
                mat.m33 = BitConverter.ToSingle(buffer, (10 * 4));
                mat.m34 = BitConverter.ToSingle(buffer, (11 * 4));

                mat.m41 = BitConverter.ToSingle(buffer, (12 * 4));
                mat.m42 = BitConverter.ToSingle(buffer, (13 * 4));
                mat.m43 = BitConverter.ToSingle(buffer, (14 * 4));
                mat.m44 = BitConverter.ToSingle(buffer, (15 * 4));
                return mat;
            }
        }

        public static Vector3 ReadHEADScatter(ulong address, int PlayerIsCrouch)
        {
            unsafe
            {
                byte[] buffer = Offsets.vmmScatter.Read(address, 12);

                //convert bytes to floats
                Vector3 vec = new Vector3();

                if (PlayerIsCrouch == 0) //CROUCH NORMAL
                {
                    _CROUCH = 65F;
                    //_CROUCH = 60F;
                }
                else if (PlayerIsCrouch == 1) //CROUCH crouched
                {
                    _CROUCH = 50F;
                }
                else if (PlayerIsCrouch == 2) //CROUCH 2 elongated
                {
                    _CROUCH = 20F;
                }

                vec.x = BitConverter.ToSingle(buffer, (0 * 4));
                vec.y = BitConverter.ToSingle(buffer, (1 * 4));
                vec.z = BitConverter.ToSingle(buffer, (2 * 4)) + _CROUCH;
                return vec;
            }
        }

        public static Vector3 ReadFOOTScatter(ulong address)
        {
            unsafe
            {
                byte[] buffer = Offsets.vmmScatter.Read(address, 12);

                //convert bytes to floats
                Vector3 vec = new Vector3();

                vec.x = BitConverter.ToSingle(buffer, (0 * 4));
                vec.y = BitConverter.ToSingle(buffer, (1 * 4));
                vec.z = BitConverter.ToSingle(buffer, (2 * 4));
                return vec;
            }
        }

        public static Vector3 ReadHEAD(ulong address, int PlayerIsCrouch)
        {
            //3 floats contiguously in memory
            byte[] buffer = new byte[3 * 4];
            int size = 12; //3 * 4 = 12
            unsafe
            {
                fixed (byte* pBuffer = buffer)
                {
                    if (Offsets.vmm.MemRead(Offsets.processPid, address, (uint)size, (nint)pBuffer, Framework.Vmm.FLAG_NOCACHE | Framework.Vmm.FLAG_NOPAGING | Framework.Vmm.FLAG_ZEROPAD_ON_FAIL | Framework.Vmm.FLAG_NOPAGING_IO) == size)
                    {
                        //convert bytes to floats
                        Vector3 vec = new Vector3();

                        if (PlayerIsCrouch == 0) //CROUCH NORMAL
                        {
                            _CROUCH = 65F;
                            //_CROUCH = 60F;
                        }
                        else if (PlayerIsCrouch == 1) //CROUCH crouched
                        {
                            _CROUCH = 50F;
                        }
                        else if (PlayerIsCrouch == 2) //CROUCH 2 elongated
                        {
                            _CROUCH = 20F;
                        }

                        vec.x = BitConverter.ToSingle(buffer, (0 * 4));
                        vec.y = BitConverter.ToSingle(buffer, (1 * 4));
                        vec.z = BitConverter.ToSingle(buffer, (2 * 4)) + _CROUCH;
                        return vec;
                    }
                    else
                    {
                        Console.WriteLine("Failed to read memory! HEAD");
                        return default;
                    }
                }
            }
        }

        public static Vector3 ReadFOOT(ulong address)
        {
            //3 floats contiguously in memory
            byte[] buffer = new byte[3 * 4];
            int size = 12; //3 * 4 = 12
            unsafe
            {
                fixed (byte* pBuffer = buffer)
                {
                    if (Offsets.vmm.MemRead(Offsets.processPid, address, (uint)size, (nint)pBuffer, Framework.Vmm.FLAG_NOCACHE | Framework.Vmm.FLAG_NOPAGING | Framework.Vmm.FLAG_ZEROPAD_ON_FAIL | Framework.Vmm.FLAG_NOPAGING_IO) == size)
                    {
                        //convert bytes to floats
                        Vector3 vec = new Vector3();
                        vec.x = BitConverter.ToSingle(buffer, (0 * 4));
                        vec.y = BitConverter.ToSingle(buffer, (1 * 4));
                        vec.z = BitConverter.ToSingle(buffer, (2 * 4));
                        return vec;
                    }
                    else
                    {
                        Console.WriteLine("Failed to read memory! FOOT");
                        return default;
                    }
                }
            }
        }

        public static void init(string gameName, string moduleName)
        {
            Console.WriteLine("Loading dma");
            Offsets.vmm = new Vmm("-device", "fpga", "-norefresh");
            //Offsets.vmm = new Vmm("-device", "fpga");

            Console.WriteLine("Finding process");

            while (true)
            {

                if (Offsets.vmm.PidGetFromName(gameName, out Offsets.processPid))
                {

                    //solve the problem of two processes with the same name
                    foreach (uint pid in Offsets.vmm.PidList())
                    {
                        //Console.Write($"{Offsets.vmm.ProcessGetInformationString(element, 3)} ");

                        if (Offsets.vmm.ProcessGetInformationString(pid, 3).Contains(Examples.Example.keyHelpFindProcess))
                        {
                            Offsets.processPid = pid;
                        }

                    }

                    Console.WriteLine("Found Game!");

                    Framework.Offsets.vmmScatter = Framework.Offsets.vmm.Scatter_Initialize(Framework.Offsets.processPid, Framework.Vmm.FLAG_NOCACHE);

                    break;
                }
                else
                {
                    Console.WriteLine("Game could not be found! Please open it and try again.");
                    Thread.Sleep(5000);
                }
            }

            Console.WriteLine("Fixing cr3");
            if (Vmm.FixCr3(Offsets.vmm, Offsets.processPid, moduleName))
            {
                Console.WriteLine("Cr3 fixed successfully");
                Vmm.MAP_MODULEENTRY Dll = Offsets.vmm.Map_GetModuleFromName(Offsets.processPid, moduleName);
                Offsets.GameAssembly = Dll.vaBase;
            }
            else
            {
                Console.WriteLine("Failed to fix cr3");
            }
        }

        public static bool Write<T>(ulong address, T data, Vmm vmm, uint processPid) where T : struct
        {
            byte[] buffer = StructureToByteArray(data);
            unsafe
            {
                fixed (byte* pBuffer = buffer)
                {
                    return vmm.MemWrite(processPid, address, (uint)buffer.Length, (nint)pBuffer);
                }
            }
        }

        private static byte[] StructureToByteArray<T>(T structure) where T : struct
        {
            int size = Marshal.SizeOf(structure);
            byte[] array = new byte[size];
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.StructureToPtr(structure, ptr, false);
                Marshal.Copy(ptr, array, 0, size);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return array;
        }

        private static T ByteArrayToStructure<T>(byte[] byteArray) where T : struct
        {
            T structure;
            int size = Marshal.SizeOf(typeof(T));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            try
            {
                Marshal.Copy(byteArray, 0, ptr, size);
                structure = Marshal.PtrToStructure<T>(ptr);
            }
            finally
            {
                Marshal.FreeHGlobal(ptr);
            }
            return structure;
        }
    }
}