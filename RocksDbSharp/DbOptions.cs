﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RocksDbSharp
{
    public enum AccessHint
    {
        None,
        Normal,
        Sequential,
        WillNeed,
    }

    // Summaries taken from:
    // rocksdb/include/rocksdb/options.h
    public class DbOptions : ColumnFamilyOptions
    {
        internal bool CreateIfMissing { get; set; }

        /// <summary>
        /// By default, RocksDB uses only one background thread for flush and
        /// compaction. Calling this function will set it up such that total of
        /// `total_threads` is used. Good value for `total_threads` is the number of
        /// cores. You almost definitely want to call this function if your system is
        /// bottlenecked by RocksDB.
        /// </summary>
        public DbOptions IncreaseParallelism(int totalThreads)
        {
            Native.Instance.rocksdb_options_increase_parallelism(Handle, totalThreads);
            return this;
        }

        /// <summary>
        /// If true, the database will be created if it is missing.
        /// Default: false
        /// </summary>
        public DbOptions SetCreateIfMissing(bool value = true)
        {
            CreateIfMissing = value; // remember this so that we can change treatment of column families during creation
            Native.Instance.rocksdb_options_set_create_if_missing(Handle, value);
            return this;
        }

        /// <summary>
        /// If true, missing column families will be automatically created.
        /// Default: false
        /// </summary>
        public DbOptions SetCreateMissingColumnFamilies(bool value = true)
        {
            Native.Instance.rocksdb_options_set_create_missing_column_families(Handle, value);
            return this;
        }

        /// <summary>
        /// If true, an error is raised if the database already exists.
        /// Default: false
        /// </summary>
        public DbOptions SetErrorIfExists(bool value = true)
        {
            Native.Instance.rocksdb_options_set_error_if_exists(Handle, value);
            return this;
        }

        /// <summary>
        /// If true, RocksDB will aggressively check consistency of the data.
        /// Also, if any of the  writes to the database fails (Put, Delete, Merge,
        /// Write), the database will switch to read-only mode and fail all other
        /// Write operations.
        /// In most cases you want this to be set to true.
        /// Default: true
        /// </summary>
        public DbOptions SetParanoidChecks(bool value = true)
        {
            Native.Instance.rocksdb_options_set_paranoid_checks(Handle, value);
            return this;
        }

        /// <summary>
        /// Use the specified object to interact with the environment,
        /// e.g. to read/write files, schedule background work, etc.
        /// Default: Env::Default()
        /// </summary>
        public DbOptions SetEnv(IntPtr env)
        {
            Native.Instance.rocksdb_options_set_env(Handle, env);
            return this;
        }

        /// <summary>
        /// Any internal progress/error information generated by the db will
        /// be written to info_log if it is non-nullptr, or to a file stored
        /// in the same directory as the DB contents if info_log is nullptr.
        /// Default: nullptr
        /// </summary>
        public DbOptions SetInfoLog(IntPtr logger)
        {
            Native.Instance.rocksdb_options_set_info_log(Handle, logger);
            return this;
        }

        /// <summary>
        /// Number of open files that can be used by the DB.  You may need to
        /// increase this if your database has a large working set. Value -1 means
        /// files opened are always kept open. You can estimate number of files based
        /// on target_file_size_base and target_file_size_multiplier for level-based
        /// compaction. For universal-style compaction, you can usually set it to -1.
        /// Default: 5000 or ulimit value of max open files (whichever is smaller)
        /// </summary>
        public DbOptions SetMaxOpenFiles(int value)
        {
            Native.Instance.rocksdb_options_set_max_open_files(Handle, value);
            return this;
        }

        /// <summary>
        /// If max_open_files is -1, DB will open all files on DB::Open(). You can
        /// use this option to increase the number of threads used to open the files.
        /// Default: 16
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public DbOptions SetMaxFileOpeningThreads(int value)
        {
            Native.Instance.rocksdb_options_set_max_file_opening_threads(Handle, value);
            return this;
        }

        /// <summary>
        /// Once write-ahead logs exceed this size, we will start forcing the flush of
        /// column families whose memtables are backed by the oldest live WAL file
        /// (i.e. the ones that are causing all the space amplification). If set to 0
        /// (default), we will dynamically choose the WAL size limit to be
        /// [sum of all write_buffer_size * max_write_buffer_number] * 4
        /// Default: 0
        /// </summary>
        public DbOptions SetMaxTotalWalSize(ulong n)
        {
            Native.Instance.rocksdb_options_set_max_total_wal_size(Handle, n);
            return this;
        }

        /// <summary>
        /// Recovery mode to control the consistency while replaying WAL
        /// Default: kPointInTimeRecovery
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        [Obsolete("Use Recovery enum")]
        public DbOptions SetWalRecoveryMode(WalRecoveryMode mode)
        {
            Native.Instance.rocksdb_options_set_wal_recovery_mode(Handle, mode);
            return this;
        }

        /// <summary>
        /// Recovery mode to control the consistency while replaying WAL
        /// Default: kPointInTimeRecovery
        /// </summary>
        /// <param name="mode"></param>
        /// <returns></returns>
        public DbOptions SetWalRecoveryMode(Recovery mode)
        {
            Native.Instance.rocksdb_options_set_wal_recovery_mode(Handle, mode);
            return this;
        }

        /// <summary>
        /// Enables statistics so that you can call GetStatisticsString() later
        /// </summary>
        /// <returns></returns>
        public DbOptions EnableStatistics()
        {
            Native.Instance.rocksdb_options_enable_statistics(Handle);
            return this;
        }
        /// <summary>
        /// Skips Status Update on DBOpen, useful for increasing DB Open time on slower disks
        /// default: false
        /// </summary>
        /// <returns></returns>
        public DbOptions SkipStatsUpdateOnOpen(bool val = false)
        {
            Native.Instance.rocksdb_options_set_skip_stats_update_on_db_open(Handle, val);
            return this;
        }

        /// <summary>
        /// Constructs and returns a string containing statistics if statistics have been enabled
        /// through EnableStatistics()
        /// </summary>
        /// <returns></returns>
        public string GetStatisticsString()
        {
            return Native.Instance.rocksdb_options_statistics_get_string_marshaled(Handle);
        }

        /// <summary>
        /// Maximum number of concurrent background compaction jobs, submitted to
        /// the default LOW priority thread pool.
        /// We first try to schedule compactions based on
        /// `base_background_compactions`. If the compaction cannot catch up , we
        /// will increase number of compaction threads up to
        /// `max_background_compactions`.
        ///
        /// If you're increasing this, also consider increasing number of threads in
        /// LOW priority thread pool. For more information, see
        /// Env::SetBackgroundThreads
        /// Default: 1
        /// </summary>
        public DbOptions SetMaxBackgroundCompactions(int value)
        {
            Native.Instance.rocksdb_options_set_max_background_compactions(Handle, value);
            return this;
        }

        /// <summary>
        /// Suggested number of concurrent background compaction jobs, submitted to
        /// the default LOW priority thread pool.
        ///
        /// Default: 1
        /// </summary>
        /// <returns></returns>
        public DbOptions SetBaseBackgroundCompactions(int value)
        {
            Native.Instance.rocksdb_options_set_base_background_compactions(Handle, value);
            return this;
        }

        /// <summary>
        /// Maximum number of concurrent background memtable flush jobs, submitted to
        /// the HIGH priority thread pool.
        ///
        /// By default, all background jobs (major compaction and memtable flush) go
        /// to the LOW priority pool. If this option is set to a positive number,
        /// memtable flush jobs will be submitted to the HIGH priority pool.
        /// It is important when the same Env is shared by multiple db instances.
        /// Without a separate pool, long running major compaction jobs could
        /// potentially block memtable flush jobs of other db instances, leading to
        /// unnecessary Put stalls.
        ///
        /// If you're increasing this, also consider increasing number of threads in
        /// HIGH priority thread pool. For more information, see
        /// Env::SetBackgroundThreads
        /// Default: 1
        /// </summary>
        public DbOptions SetMaxBackgroundFlushes(int value)
        {
            Native.Instance.rocksdb_options_set_max_background_flushes(Handle, value);
            return this;
        }

        /// <summary>
        /// Specify the maximal size of the info log file. If the log file
        /// is larger than `max_log_file_size`, a new info log file will
        /// be created.
        /// If max_log_file_size == 0, all logs will be written to one
        /// log file.
        /// </summary>
        public DbOptions SetMaxLogFileSize(ulong value)
        {
            Native.Instance.rocksdb_options_set_max_log_file_size(Handle, value);
            return this;
        }

        /// <summary>
        /// Time for the info log file to roll (in seconds).
        /// If specified with non-zero value, log file will be rolled
        /// if it has been active longer than `log_file_time_to_roll`.
        /// Default: 0 (disabled)
        /// </summary>
        public DbOptions SetLogFileTimeToRoll(ulong value)
        {
            Native.Instance.rocksdb_options_set_log_file_time_to_roll(Handle, value);
            return this;
        }

        /// <summary>
        /// Maximal info log files to be kept.
        /// Default: 1000
        /// </summary>
        public DbOptions SetKeepLogFileNum(ulong value)
        {
            Native.Instance.rocksdb_options_set_keep_log_file_num(Handle, value);
            return this;
        }

        /// <summary>
        /// Recycle log files.
        /// If non-zero, we will reuse previously written log files for new
        /// logs, overwriting the old data.  The value indicates how many
        /// such files we will keep around at any point in time for later
        /// use.  This is more efficient because the blocks are already
        /// allocated and fdatasync does not need to update the inode after
        /// each write.
        /// Default: 0
        /// </summary>
        public DbOptions SetRecycleLogFileNum(ulong value)
        {
            Native.Instance.rocksdb_options_set_recycle_log_file_num(Handle, value);
            return this;
        }

        /// <summary>
        /// manifest file is rolled over on reaching this limit.
        /// The older manifest file be deleted.
        /// The default value is MAX_INT so that roll-over does not take place.
        /// </summary>
        public DbOptions SetMaxManifestFileSize(ulong value)
        {
            Native.Instance.rocksdb_options_set_max_manifest_file_size(Handle, value);
            return this;
        }

        /// <summary>
        /// Number of shards used for table cache.
        /// </summary>
        public DbOptions SetTableCacheNumShardbits(int value)
        {
            Native.Instance.rocksdb_options_set_table_cache_numshardbits(Handle, value);
            return this;
        }

        // DEPRECATED
        [Obsolete]
        public DbOptions SetTableCacheRemoveScanCountLimit(int value)
        {
            Native.Instance.rocksdb_options_set_table_cache_remove_scan_count_limit(Handle, value);
            return this;
        }

        /// <summary>
        /// If true, then every store to stable storage will issue a fsync.
        /// If false, then every store to stable storage will issue a fdatasync.
        /// This parameter should be set to true while storing data to
        /// filesystem like ext3 that can lose files after a reboot.
        /// Default: false
        /// </summary>
        public DbOptions SetUseFsync(int value)
        {
            Native.Instance.rocksdb_options_set_use_fsync(Handle, value);
            return this;
        }

        /// <summary>
        /// This specifies the info LOG dir.
        /// If it is empty, the log files will be in the same dir as data.
        /// If it is non empty, the log files will be in the specified dir,
        /// and the db data dir's absolute path will be used as the log file
        /// name's prefix.
        /// </summary>
        public DbOptions SetDbLogDir(string value)
        {
            Native.Instance.rocksdb_options_set_db_log_dir(Handle, value);
            return this;
        }

        /// <summary>
        /// This specifies the absolute dir path for write-ahead logs (WAL).
        /// If it is empty, the log files will be in the same dir as data,
        ///   dbname is used as the data dir by default
        /// If it is non empty, the log files will be in kept the specified dir.
        /// When destroying the db,
        ///   all log files in wal_dir and the dir itself is deleted
        /// </summary>
        public DbOptions SetWalDir(string value)
        {
            Native.Instance.rocksdb_options_set_wal_dir(Handle, value);
            return this;
        }

        /// <summary>
        /// The following two fields affect how archived logs will be deleted.
        /// 1. If both set to 0, logs will be deleted asap and will not get into
        ///    the archive.
        /// 2. If WAL_ttl_seconds is 0 and WAL_size_limit_MB is not 0,
        ///    WAL files will be checked every 10 min and if total size is greater
        ///    then WAL_size_limit_MB, they will be deleted starting with the
        ///    earliest until size_limit is met. All empty files will be deleted.
        /// 3. If WAL_ttl_seconds is not 0 and WAL_size_limit_MB is 0, then
        ///    WAL files will be checked every WAL_ttl_secondsi / 2 and those that
        ///    are older than WAL_ttl_seconds will be deleted.
        /// 4. If both are not 0, WAL files will be checked every 10 min and both
        ///    checks will be performed with ttl being first.
        /// </summary>
        public DbOptions SetWALTtlSeconds(ulong value)
        {
            Native.Instance.rocksdb_options_set_WAL_ttl_seconds(Handle, value);
            return this;
        }

        /// <summary>
        /// The following two fields affect how archived logs will be deleted.
        /// 1. If both set to 0, logs will be deleted asap and will not get into
        ///    the archive.
        /// 2. If WAL_ttl_seconds is 0 and WAL_size_limit_MB is not 0,
        ///    WAL files will be checked every 10 min and if total size is greater
        ///    then WAL_size_limit_MB, they will be deleted starting with the
        ///    earliest until size_limit is met. All empty files will be deleted.
        /// 3. If WAL_ttl_seconds is not 0 and WAL_size_limit_MB is 0, then
        ///    WAL files will be checked every WAL_ttl_secondsi / 2 and those that
        ///    are older than WAL_ttl_seconds will be deleted.
        /// 4. If both are not 0, WAL files will be checked every 10 min and both
        ///    checks will be performed with ttl being first.
        /// </summary>
        public DbOptions SetWALSizeLimitMB(ulong value)
        {
            Native.Instance.rocksdb_options_set_WAL_size_limit_MB(Handle, value);
            return this;
        }

        /// <summary>
        /// Number of bytes to preallocate (via fallocate) the manifest
        /// files.  Default is 4mb, which is reasonable to reduce random IO
        /// as well as prevent overallocation for mounts that preallocate
        /// large amounts of data (such as xfs's allocsize option).
        /// </summary>
        public DbOptions SetManifestPreallocationSize(ulong value)
        {
            Native.Instance.rocksdb_options_set_manifest_preallocation_size(Handle, value);
            return this;
        }

        /// <summary>
        /// Allow the OS to mmap file for reading sst tables. Default: false
        /// </summary>
        public DbOptions SetAllowMmapReads(bool value)
        {
            Native.Instance.rocksdb_options_set_allow_mmap_reads(Handle, value);
            return this;
        }

        /// <summary>
        /// Allow the OS to mmap file for writing.
        /// DB::SyncWAL() only works if this is set to false.
        /// Default: false
        /// </summary>
        public DbOptions SetAllowMmapWrites(bool value)
        {
            Native.Instance.rocksdb_options_set_allow_mmap_writes(Handle, value);
            return this;
        }

        /// <summary>
        /// Enable direct I/O mode for read/write
        /// they may or may not improve performance depending on the use case
        ///
        /// Files will be opened in "direct I/O" mode
        /// which means that data r/w from the disk will not be cached or
        /// bufferized. The hardware buffer of the devices may however still
        /// be used. Memory mapped files are not impacted by these parameters.
        /// Use O_DIRECT for reading file
        /// Default: false
        /// </summary>
        public DbOptions SetUseDirectReads(bool value)
        {
            Native.Instance.rocksdb_options_set_use_direct_reads(Handle, value);
            return this;
        }

        /// <summary>
        /// Use O_DIRECT for both reads and writes in background flush and compactions
        /// When true, we also force new_table_reader_for_compaction_inputs to true.
        /// Default: false
        /// Not supported in ROCKSDB_LITE mode!
        /// Default: false
        /// </summary>
        public DbOptions SetUseDirectIoForFlushAndCompaction(bool value)
        {
            Native.Instance.rocksdb_options_set_use_direct_io_for_flush_and_compaction(Handle, value);
            return this;
        }

        /// <summary>
        /// Disable child process inherit open files. Default: true
        /// </summary>
        public DbOptions SetIsFdCloseOnExec(bool value)
        {
            Native.Instance.rocksdb_options_set_is_fd_close_on_exec(Handle, value);
            return this;
        }

        /// <summary>
        /// if not zero, dump rocksdb.stats to LOG every stats_dump_period_sec
        /// Default: 600 (10 min)
        /// </summary>
        public DbOptions SetStatsDumpPeriodSec(uint value)
        {
            Native.Instance.rocksdb_options_set_stats_dump_period_sec(Handle, value);
            return this;
        }

        /// <summary>
        /// If set true, will hint the underlying file system that the file
        /// access pattern is random, when a sst file is opened.
        /// Default: true
        /// </summary>
        public DbOptions SetAdviseRandomOnOpen(bool value)
        {
            Native.Instance.rocksdb_options_set_advise_random_on_open(Handle, value);
            return this;
        }

        /// <summary>
        /// Amount of data to build up in memtables across all column
        /// families before writing to disk.
        ///
        /// This is distinct from write_buffer_size, which enforces a limit
        /// for a single memtable.
        ///
        /// This feature is disabled by default. Specify a non-zero value
        /// to enable it.
        ///
        /// Default: 0 (disabled)
        /// </summary>
        public DbOptions SetDbWriteBufferSize(ulong size)
        {
            Native.Instance.rocksdb_options_set_db_write_buffer_size(Handle, size);
            return this;
        }

        /// <summary>
        /// Specify the file access pattern once a compaction is started.
        /// It will be applied to all input files of a compaction.
        /// Default: NORMAL
        /// </summary>
        public DbOptions SetAccessHintOnCompactionStart(int value)
        {
            Native.Instance.rocksdb_options_set_access_hint_on_compaction_start(Handle, value);
            return this;
        }

        /// <summary>
        /// Use adaptive mutex, which spins in the user space before resorting
        /// to kernel. This could reduce context switch when the mutex is not
        /// heavily contended. However, if the mutex is hot, we could end up
        /// wasting spin time.
        /// Default: false
        /// </summary>
        public DbOptions SetUseAdaptiveMutex(bool value)
        {
            Native.Instance.rocksdb_options_set_use_adaptive_mutex(Handle, value);
            return this;
        }

        /// <summary>
        /// Allows OS to incrementally sync files to disk while they are being
        /// written, asynchronously, in the background. This operation can be used
        /// to smooth out write I/Os over time. Users shouldn't reply on it for
        /// persistency guarantee.
        /// Issue one request for every bytes_per_sync written. 0 turns it off.
        /// Default: 0
        ///
        /// You may consider using rate_limiter to regulate write rate to device.
        /// When rate limiter is enabled, it automatically enables bytes_per_sync
        /// to 1MB.
        ///
        /// This option applies to table files
        /// </summary>
        public DbOptions SetBytesPerSync(ulong value)
        {
            Native.Instance.rocksdb_options_set_bytes_per_sync(Handle, value);
            return this;
        }

        /// <summary>
        /// If true, allow multi-writers to update mem tables in parallel.
        /// Only some memtable_factory-s support concurrent writes; currently it
        /// is implemented only for SkipListFactory.  Concurrent memtable writes
        /// are not compatible with inplace_update_support or filter_deletes.
        /// It is strongly recommended to set enable_write_thread_adaptive_yield
        /// if you are going to use this feature.
        ///
        /// Default: true
        /// </summary>
        /// <returns></returns>
        public DbOptions SetAllowConcurrentMemtableWrite(bool value)
        {
            Native.Instance.rocksdb_options_set_allow_concurrent_memtable_write(Handle, value);
            return this;
        }

        /// <summary>
        /// If true, threads synchronizing with the write batch group leader will
        /// wait for up to write_thread_max_yield_usec before blocking on a mutex.
        /// This can substantially improve throughput for concurrent workloads,
        /// regardless of whether allow_concurrent_memtable_write is enabled.
        ///
        /// Default: true
        /// </summary>
        /// <returns></returns>
        public DbOptions SetEnableWriteThreadAdaptiveYield(bool value)
        {
            Native.Instance.rocksdb_options_set_enable_write_thread_adaptive_yield(Handle, value);
            return this;
        }

        /// <summary>
        /// The periodicity when obsolete files get deleted. The default
        /// value is 6 hours. The files that get out of scope by compaction
        /// process will still get automatically delete on every compaction,
        /// regardless of this setting
        /// </summary>
        public DbOptions SetDeleteObsoleteFilesPeriodMicros(ulong value)
        {
            Native.Instance.rocksdb_options_set_delete_obsolete_files_period_micros(Handle, value);
            return this;
        }

        /// <summary>
        /// Set appropriate parameters for bulk loading.
        /// The reason that this is a function that returns "this" instead of a
        /// constructor is to enable chaining of multiple similar calls in the future.
        ///
        /// All data will be in level 0 without any automatic compaction.
        /// It's recommended to manually call CompactRange(NULL, NULL) before reading
        /// from the database, because otherwise the read can be very slow.
        /// </summary>
        public DbOptions PrepareForBulkLoad()
        {
            Native.Instance.rocksdb_options_prepare_for_bulk_load(Handle);
            return this;
        }

    }
}
