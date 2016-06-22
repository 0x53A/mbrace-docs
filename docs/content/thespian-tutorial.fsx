(*** hide ***)
// This block of code is omitted in the generated HTML documentation. Use 
// it to define helpers that you do not want to show in the documentation.
#load "../../packages/MBrace.Thespian/MBrace.Thespian.fsx"


(**

# Using MBrace with a Locally Simulated Cluster


To use a locally simulated cluster, 

1. [Download or clone the starter pack](https://github.com/mbraceproject/MBrace.StarterKit/blob/master/mbrace-versions.md).
2. Build the solution to get the required packages.
3. Open the first tutorial script and to use MBrace programming with your simulated cluster.

The scripts follow the tutorials in the [Core Programming Model](programming-model.html).

The locally simulated cloud fabric is called "MBrace.Thespian".  This utilizes the multi-core capabillities of your
machine and is independent of any particular cloud provider, but supports the
same programming model as MBrace.Azure and other MBrace implementations.  This lets
you learn the MBrace cloud programming model in a provider-independent way.


## Initializing Thespian Manually

Your cluster workers will be created automaticaly when using the scripts in the starter pack. You can also
initialize manually (e.g. from an application) as follows (you may need to change the path to the worker exe):
*)

open MBrace.Thespian
ThespianWorker.LocalExecutable <- "packages/mbrace.thespian/tools/mbrace.thespian.worker.exe"
let cluster = 
    ThespianCluster.InitOnCurrentMachine(workerCount = 4, 
                                         logger = ConsoleLogger(), 
                                         logLevel = LogLevel.Info)

(**
The above script creates a cluster on the local machine with 4 worker instances.
You can create a multi-machine cluster using instances of ``ThespianWorker`` and ``InitOnWorker``.

### Multi-machine cluster

First you need to create a cluster on the machine you want to be the master:
*)
(** The master needs to run on a well-known uri so that the workers can later connect to it *)
let hostname = Environment.MachineNam
let port = 7890

(** To use cloud files, you need the share a directory which is available to all worker instances somewhere *)
let cloudFileStore = FileSystemStore.Create("\\servername\share)

let cluster =
    let worker = ThespianWorker.Spawn(hostname, port)
    ThespianCluster.InitOnWorker(worker, cloudFileStore)
let uri = cluster.Uri
printfn "Created new cluster on %s" uri

(**
Now that you have created the cluster, you need to attach workers to it.
To do so, run this on all machines:
*)

let cluster = ThespianCluster.Connect(uri)
cluster.AttachNewLocalWorkers(4)
